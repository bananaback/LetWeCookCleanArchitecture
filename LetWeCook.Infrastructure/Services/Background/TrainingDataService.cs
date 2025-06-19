using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Common;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;
using LetWeCook.Domain.Events;
using LetWeCook.Domain.Utilities;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LetWeCook.Infrastructure.Services.Background;

public class TrainingDataService : BackgroundService
{
    private readonly ILogger<TrainingDataService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _pythonServerUrl;
    private const string RecipeFeaturesCsvPath = "recipe-features.csv";
    private const string InteractionsCsvPath = "interactions.csv";
    private const string UserFeaturesCsvPath = "user_features.csv";
    private const string TrainingDataCsvPath = "training_data.csv";
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    public TrainingDataService(
        ILogger<TrainingDataService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _pythonServerUrl = configuration.GetValue<string>("PredictionService:ApiUrl")
            ?? throw new InvalidOperationException("PredictionService:ApiUrl is not configured in appsettings.");
    }

    private async Task SendFileToPythonServer(string filePath, string fileName, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        using var form = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(filePath);
        var content = new StreamContent(fileStream);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");

        form.Add(content, "file", fileName);

        var response = await client.PostAsync($"{_pythonServerUrl}/upload", form, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Upload failed for {fileName}: {response.StatusCode} - {error}");
        }

        var success = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Upload successful for {FileName}: {Response}", fileName, success);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Training data service is running at: {time}", DateTimeOffset.Now);

                var config = TrainingDataConfigHelper.LoadConfig();
                _logger.LogInformation("Last CSV upload: {CsvTime}, Last Snapshot: {SnapTime}",
                    config.TrainingDataCsvLastSent, config.RecipesSnapshotLastSent);

                // Handle CSV Upload
                if (TrainingDataConfigHelper.ShouldUploadCsv())
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var recipeRepository = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
                        var userInteractionRepository = scope.ServiceProvider.GetRequiredService<IUserInteractionRepository>();
                        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                        var donationRepository = scope.ServiceProvider.GetRequiredService<IDonationRepository>();
                        var ratingRepository = scope.ServiceProvider.GetRequiredService<IRecipeRatingRepository>();
                        var suggestionFeedbackRepository = scope.ServiceProvider.GetRequiredService<ISuggestionFeedbackRepository>();

                        // 1. Generate recipe-features.csv
                        var recipeRecords = new List<RecipeFeatureRow>();
                        var recipes = await recipeRepository.GetAllAsync(stoppingToken);
                        foreach (var recipe in recipes)
                        {
                            var detailedRecipe = await recipeRepository.GetRecipeDetailsByIdAsync(recipe.Id, stoppingToken);
                            var logRow = CreateRecipeFeatureRow(detailedRecipe);
                            recipeRecords.Add(logRow);
                        }
                        using (var writer = new StreamWriter(RecipeFeaturesCsvPath, false, Encoding.UTF8))
                        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                        {
                            csv.WriteRecords(recipeRecords);
                        }
                        _logger.LogInformation("Recipe features CSV written to {CsvPath}", RecipeFeaturesCsvPath);

                        // 2. Generate interactions.csv
                        var interactionRecords = await userInteractionRepository.GetAggregatedInteractionsAsync();
                        using (var writer = new StreamWriter(InteractionsCsvPath, false, Encoding.UTF8))
                        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                        {
                            csv.WriteRecords(interactionRecords);
                        }
                        _logger.LogInformation("Interactions CSV written to {CsvPath}", InteractionsCsvPath);

                        // 3. Generate user_features.csv
                        var userRecords = new List<UserFeatureRow>();
                        var users = await userRepository.GetAllWithProfileAsync(stoppingToken);
                        foreach (var user in users.Where(u => u.Profile != null && !u.IsRemoved))
                        {
                            var profile = user.Profile!;
                            var age = CalculateAge(profile.BirthDate);
                            var dietaryPrefs = profile.DietaryPreferences.ToDictionary(p => p.Name, p => true);

                            var userRow = new UserFeatureRow
                            {
                                UserId = user.Id,
                                Age = age,
                                Gender_Male = profile.Gender == Gender.Male,
                                Gender_Female = profile.Gender == Gender.Female,
                                Gender_Other = profile.Gender == Gender.Other,
                                Pref_Vegetarian = dietaryPrefs.ContainsKey("Vegetarian"),
                                Pref_Vegan = dietaryPrefs.ContainsKey("Vegan"),
                                Pref_GlutenFree = dietaryPrefs.ContainsKey("GlutenFree"),
                                Pref_Pescatarian = dietaryPrefs.ContainsKey("Pescatarian"),
                                Pref_LowCalorie = dietaryPrefs.ContainsKey("LowCalorie"),
                                Pref_HighProtein = dietaryPrefs.ContainsKey("HighProtein"),
                                Pref_LowCarb = dietaryPrefs.ContainsKey("LowCarb"),
                                Pref_LowFat = dietaryPrefs.ContainsKey("LowFat"),
                                Pref_LowSugar = dietaryPrefs.ContainsKey("LowSugar"),
                                Pref_HighFiber = dietaryPrefs.ContainsKey("HighFiber"),
                                Pref_LowSodium = dietaryPrefs.ContainsKey("LowSodium")
                            };
                            userRecords.Add(userRow);
                        }
                        using (var writer = new StreamWriter(UserFeaturesCsvPath, false, Encoding.UTF8))
                        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                        {
                            csv.WriteRecords(userRecords);
                        }
                        _logger.LogInformation("User features CSV written to {CsvPath}", UserFeaturesCsvPath);

                        // 4. Generate training_data.csv
                        var trainingRecords = new List<TrainingLogRow>();
                        var donations = await donationRepository.GetAllAsync(stoppingToken);
                        var ratings = await ratingRepository.GetAllAsync(stoppingToken);
                        var suggestionFeedbacks = await suggestionFeedbackRepository.GetAllAsync(stoppingToken);

                        // Ratings
                        foreach (var rating in ratings)
                        {
                            var user = await userRepository.GetWithProfileByIdAsync(rating.UserId, stoppingToken);
                            if (user?.Profile == null || !user.Profile.DietaryPreferences.Any())
                            {
                                _logger.LogWarning("Skipping rating: user/profile missing for ID {UserId}.", rating.UserId);
                                continue;
                            }

                            var recipe = await recipeRepository.GetRecipeDetailsByIdAsync(rating.RecipeId);
                            if (recipe == null)
                            {
                                _logger.LogWarning("Skipping rating: recipe not found for ID {RecipeId}.", rating.RecipeId);
                                continue;
                            }

                            var record = CreateTrainingLogRow(user, recipe, rating.CreatedAt);
                            record.Rating = rating.Rating;
                            record.CommentLength = rating.Comment?.Length ?? 0;
                            trainingRecords.Add(record);
                        }

                        // Donations
                        foreach (var donation in donations)
                        {
                            var recipe = await recipeRepository.GetByIdAsync(donation.RecipeId);
                            if (recipe == null) continue;

                            var donator = await userRepository.GetWithProfileByIdAsync(donation.DonatorId, stoppingToken);
                            if (donator?.Profile == null || !donator.Profile.DietaryPreferences.Any())
                            {
                                _logger.LogWarning("Skipping donation: profile missing for ID {DonatorId}.", donation.DonatorId);
                                continue;
                            }

                            var record = CreateTrainingLogRow(donator, recipe, donation.CreatedAt);
                            record.DonatedAmount = donation.Amount;
                            trainingRecords.Add(record);
                        }

                        // Feedback
                        foreach (var feedback in suggestionFeedbacks)
                        {
                            var user = await userRepository.GetWithProfileByIdAsync(feedback.UserId, stoppingToken);
                            if (user?.Profile == null || !user.Profile.DietaryPreferences.Any())
                            {
                                _logger.LogWarning("Skipping feedback: profile missing for ID {UserId}.", feedback.UserId);
                                continue;
                            }

                            var record = CreateTrainingLogRow(user, feedback.Recipe, feedback.CreatedAt);
                            record.Liked = feedback.LikeOrDislike;
                            trainingRecords.Add(record);
                        }

                        // Write training_data.csv
                        using (var writer = new StreamWriter(TrainingDataCsvPath, false, Encoding.UTF8))
                        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                        {
                            csv.WriteRecords(trainingRecords);
                        }
                        _logger.LogInformation("Training data CSV written to {CsvPath}", TrainingDataCsvPath);

                        // Upload all CSVs
                        await SendFileToPythonServer(RecipeFeaturesCsvPath, "recipe-features.csv", stoppingToken);
                        await SendFileToPythonServer(InteractionsCsvPath, "interactions.csv", stoppingToken);
                        await SendFileToPythonServer(UserFeaturesCsvPath, "user_features.csv", stoppingToken);
                        await SendFileToPythonServer(TrainingDataCsvPath, "training_data.csv", stoppingToken);
                        TrainingDataConfigHelper.UpdateLastUploadTime();
                        _logger.LogInformation("All CSVs uploaded and timestamp updated.");
                    }
                }
                else
                {
                    _logger.LogInformation("CSV upload skipped (within 6-hour interval).");
                }

                // Handle Snapshot Upload
                if (TrainingDataConfigHelper.ShouldUploadSnapshot())
                {
                    _logger.LogInformation("Dispatching RecipeSnapshotRequestedEvent...");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var domainEventDispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();
                        var recipeSnapshotRequestedEvent = new RecipeSnapshotRequestedEvent();

                        await domainEventDispatcher.DispatchEventsAsync(
                            new List<DomainEvent> { recipeSnapshotRequestedEvent }, stoppingToken);
                    }

                    TrainingDataConfigHelper.UpdateLastSnapshotUploadTime();
                    _logger.LogInformation("Snapshot event dispatched and timestamp updated.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing training data.");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private RecipeFeatureRow CreateRecipeFeatureRow(Recipe recipe)
    {
        var nutrition = RecipeNutritionCalculator.CalculateRecipeNutrition(recipe);

        return new RecipeFeatureRow
        {
            RecipeId = recipe.Id,
            Calories = nutrition.Calories,
            Protein = nutrition.Protein,
            Carbohydrates = nutrition.Carbohydrates,
            Fat = nutrition.Fat,
            Sugar = nutrition.Sugar,
            Fiber = nutrition.Fiber,
            Sodium = nutrition.Sodium,
            Tag_Vietnamese = recipe.Tags.Select(t => t.Name).Contains("Vietnamese"),
            Tag_Brazilian = recipe.Tags.Select(t => t.Name).Contains("Brazilian"),
            Tag_Chinese = recipe.Tags.Select(t => t.Name).Contains("Chinese"),
            Tag_Caribbean = recipe.Tags.Select(t => t.Name).Contains("Caribbean"),
            Tag_Mexican = recipe.Tags.Select(t => t.Name).Contains("Mexican"),
            Tag_African = recipe.Tags.Select(t => t.Name).Contains("African"),
            Tag_Italian = recipe.Tags.Select(t => t.Name).Contains("Italian"),
            Tag_German = recipe.Tags.Select(t => t.Name).Contains("German"),
            Tag_Indian = recipe.Tags.Select(t => t.Name).Contains("Indian"),
            Tag_Thai = recipe.Tags.Select(t => t.Name).Contains("Thai"),
            Tag_Japanese = recipe.Tags.Select(t => t.Name).Contains("Japanese"),
            Tag_Korean = recipe.Tags.Select(t => t.Name).Contains("Korean"),
            Tag_American = recipe.Tags.Select(t => t.Name).Contains("American"),
            Tag_French = recipe.Tags.Select(t => t.Name).Contains("French"),
            Tag_Mediterranean = recipe.Tags.Select(t => t.Name).Contains("Mediterranean"),
            Tag_MiddleEastern = recipe.Tags.Select(t => t.Name).Contains("MiddleEastern"),
            Tag_Spanish = recipe.Tags.Select(t => t.Name).Contains("Spanish"),
            Tag_Greek = recipe.Tags.Select(t => t.Name).Contains("Greek"),
            Tag_Turkish = recipe.Tags.Select(t => t.Name).Contains("Turkish"),
            Meal_Main = recipe.MealCategory == MealCategory.Main,
            Meal_Side = recipe.MealCategory == MealCategory.Side,
            Meal_Soup = recipe.MealCategory == MealCategory.Soup,
            Meal_Dessert = recipe.MealCategory == MealCategory.Dessert,
            Meal_Beverage = recipe.MealCategory == MealCategory.Beverage,
            Diff_Easy = recipe.DifficultyLevel == DifficultyLevel.EASY,
            Diff_Medium = recipe.DifficultyLevel == DifficultyLevel.MEDIUM,
            Diff_Hard = recipe.DifficultyLevel == DifficultyLevel.HARD
        };
    }

    private TrainingLogRow CreateTrainingLogRow(SiteUser user, Recipe recipe, DateTime timestamp)
    {
        var nutrition = RecipeNutritionCalculator.CalculateRecipeNutrition(recipe);

        return new TrainingLogRow
        {
            UserId = user.Id,
            RecipeId = recipe.Id,
            Timestamp = timestamp,
            ViewsCount = recipe.TotalViews,
            Pref_Vegetarian = user.Profile.DietaryPreferences.Select(d => d.Name).Contains("Vegetarian"),
            Pref_Vegan = user.Profile.DietaryPreferences.Select(d => d.Name).Contains("Vegan"),
            Pref_GlutenFree = user.Profile.DietaryPreferences.Select(d => d.Name).Contains("GlutenFree"),
            Pref_Pescatarian = user.Profile.DietaryPreferences.Select(d => d.Name).Contains("Pescatarian"),
            Pref_LowCalorie = user.Profile.DietaryPreferences.Select(d => d.Name).Contains("LowCalorie"),
            Pref_HighProtein = user.Profile.DietaryPreferences.Select(d => d.Name).Contains("HighProtein"),
            Pref_LowCarb = user.Profile.DietaryPreferences.Select(d => d.Name).Contains("LowCarb"),
            Pref_LowFat = user.Profile.DietaryPreferences.Select(d => d.Name).Contains("LowFat"),
            Pref_LowSugar = user.Profile.DietaryPreferences.Select(d => d.Name).Contains("LowSugar"),
            Pref_HighFiber = user.Profile.DietaryPreferences.Select(d => d.Name).Contains("HighFiber"),
            Pref_LowSodium = user.Profile.DietaryPreferences.Select(d => d.Name).Contains("LowSodium"),
            Calories = nutrition.Calories,
            Protein = nutrition.Protein,
            Carbohydrates = nutrition.Carbohydrates,
            Fat = nutrition.Fat,
            Sugar = nutrition.Sugar,
            Fiber = nutrition.Fiber,
            Sodium = nutrition.Sodium,
            Tag_Vietnamese = recipe.Tags.Select(t => t.Name).Contains("Vietnamese"),
            Tag_Brazilian = recipe.Tags.Select(t => t.Name).Contains("Brazilian"),
            Tag_Chinese = recipe.Tags.Select(t => t.Name).Contains("Chinese"),
            Tag_Caribbean = recipe.Tags.Select(t => t.Name).Contains("Caribbean"),
            Tag_Mexican = recipe.Tags.Select(t => t.Name).Contains("Mexican"),
            Tag_African = recipe.Tags.Select(t => t.Name).Contains("African"),
            Tag_Italian = recipe.Tags.Select(t => t.Name).Contains("Italian"),
            Tag_German = recipe.Tags.Select(t => t.Name).Contains("German"),
            Tag_Indian = recipe.Tags.Select(t => t.Name).Contains("Indian"),
            Tag_Thai = recipe.Tags.Select(t => t.Name).Contains("Thai"),
            Tag_Japanese = recipe.Tags.Select(t => t.Name).Contains("Japanese"),
            Tag_Korean = recipe.Tags.Select(t => t.Name).Contains("Korean"),
            Tag_American = recipe.Tags.Select(t => t.Name).Contains("American"),
            Tag_French = recipe.Tags.Select(t => t.Name).Contains("French"),
            Tag_Mediterranean = recipe.Tags.Select(t => t.Name).Contains("Mediterranean"),
            Tag_MiddleEastern = recipe.Tags.Select(t => t.Name).Contains("MiddleEastern"),
            Tag_Spanish = recipe.Tags.Select(t => t.Name).Contains("Spanish"),
            Tag_Greek = recipe.Tags.Select(t => t.Name).Contains("Greek"),
            Tag_Turkish = recipe.Tags.Select(t => t.Name).Contains("Turkish"),
            Meal_Main = recipe.MealCategory == MealCategory.Main,
            Meal_Side = recipe.MealCategory == MealCategory.Side,
            Meal_Soup = recipe.MealCategory == MealCategory.Soup,
            Meal_Dessert = recipe.MealCategory == MealCategory.Dessert,
            Meal_Beverage = recipe.MealCategory == MealCategory.Beverage,
            Diff_Easy = recipe.DifficultyLevel == DifficultyLevel.EASY,
            Diff_Medium = recipe.DifficultyLevel == DifficultyLevel.MEDIUM,
            Diff_Hard = recipe.DifficultyLevel == DifficultyLevel.HARD
        };
    }

    private int CalculateAge(DateTime birthDate)
    {
        var today = new DateTime(2025, 6, 18); // Current date as per system
        int age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}

internal class RecipeFeatureRow
{
    public Guid RecipeId { get; set; }
    public float? Calories { get; set; }
    public float? Protein { get; set; }
    public float? Carbohydrates { get; set; }
    public float? Fat { get; set; }
    public float? Sugar { get; set; }
    public float? Fiber { get; set; }
    public float? Sodium { get; set; }
    public bool Tag_Vietnamese { get; set; }
    public bool Tag_Brazilian { get; set; }
    public bool Tag_Chinese { get; set; }
    public bool Tag_Caribbean { get; set; }
    public bool Tag_Mexican { get; set; }
    public bool Tag_African { get; set; }
    public bool Tag_Italian { get; set; }
    public bool Tag_German { get; set; }
    public bool Tag_Indian { get; set; }
    public bool Tag_Thai { get; set; }
    public bool Tag_Japanese { get; set; }
    public bool Tag_Korean { get; set; }
    public bool Tag_American { get; set; }
    public bool Tag_French { get; set; }
    public bool Tag_Mediterranean { get; set; }
    public bool Tag_MiddleEastern { get; set; }
    public bool Tag_Spanish { get; set; }
    public bool Tag_Greek { get; set; }
    public bool Tag_Turkish { get; set; }
    public bool Meal_Main { get; set; }
    public bool Meal_Side { get; set; }
    public bool Meal_Soup { get; set; }
    public bool Meal_Dessert { get; set; }
    public bool Meal_Beverage { get; set; }
    public bool Diff_Easy { get; set; }
    public bool Diff_Medium { get; set; }
    public bool Diff_Hard { get; set; }
}

internal class UserFeatureRow
{
    public Guid UserId { get; set; }
    public int Age { get; set; }
    public bool Gender_Male { get; set; }
    public bool Gender_Female { get; set; }
    public bool Gender_Other { get; set; }
    public bool Pref_Vegetarian { get; set; }
    public bool Pref_Vegan { get; set; }
    public bool Pref_GlutenFree { get; set; }
    public bool Pref_Pescatarian { get; set; }
    public bool Pref_LowCalorie { get; set; }
    public bool Pref_HighProtein { get; set; }
    public bool Pref_LowCarb { get; set; }
    public bool Pref_LowFat { get; set; }
    public bool Pref_LowSugar { get; set; }
    public bool Pref_HighFiber { get; set; }
    public bool Pref_LowSodium { get; set; }
}

internal class TrainingLogRow
{
    public Guid UserId { get; set; }
    public Guid RecipeId { get; set; }
    public DateTime Timestamp { get; set; }
    public bool? Liked { get; set; }
    public int? Rating { get; set; }
    public int? CommentLength { get; set; }
    public decimal DonatedAmount { get; set; }
    public int ViewsCount { get; set; }
    public bool Pref_Vegetarian { get; set; }
    public bool Pref_Vegan { get; set; }
    public bool Pref_GlutenFree { get; set; }
    public bool Pref_Pescatarian { get; set; }
    public bool Pref_LowCalorie { get; set; }
    public bool Pref_HighProtein { get; set; }
    public bool Pref_LowCarb { get; set; }
    public bool Pref_LowFat { get; set; }
    public bool Pref_LowSugar { get; set; }
    public bool Pref_HighFiber { get; set; }
    public bool Pref_LowSodium { get; set; }
    public float? Calories { get; set; }
    public float? Protein { get; set; }
    public float? Carbohydrates { get; set; }
    public float? Fat { get; set; }
    public float? Sugar { get; set; }
    public float? Fiber { get; set; }
    public float? Sodium { get; set; }
    public bool Tag_Vietnamese { get; set; }
    public bool Tag_Brazilian { get; set; }
    public bool Tag_Chinese { get; set; }
    public bool Tag_Caribbean { get; set; }
    public bool Tag_Mexican { get; set; }
    public bool Tag_African { get; set; }
    public bool Tag_Italian { get; set; }
    public bool Tag_German { get; set; }
    public bool Tag_Indian { get; set; }
    public bool Tag_Thai { get; set; }
    public bool Tag_Japanese { get; set; }
    public bool Tag_Korean { get; set; }
    public bool Tag_American { get; set; }
    public bool Tag_French { get; set; }
    public bool Tag_Mediterranean { get; set; }
    public bool Tag_MiddleEastern { get; set; }
    public bool Tag_Spanish { get; set; }
    public bool Tag_Greek { get; set; }
    public bool Tag_Turkish { get; set; }
    public bool Meal_Main { get; set; }
    public bool Meal_Side { get; set; }
    public bool Meal_Soup { get; set; }
    public bool Meal_Dessert { get; set; }
    public bool Meal_Beverage { get; set; }
    public bool Diff_Easy { get; set; }
    public bool Diff_Medium { get; set; }
    public bool Diff_Hard { get; set; }
}