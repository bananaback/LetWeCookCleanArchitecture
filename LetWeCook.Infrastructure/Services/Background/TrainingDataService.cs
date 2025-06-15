using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using LetWeCook.Application.Dtos;
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
    private const string TrainingCsvPath = "recipe-features.csv";
    private const string InteractionsCsvPath = "interactions.csv";
    private const string UserFeaturesCsvPath = "user_features.csv";
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);
    private string _pythonServerUrl = string.Empty;

    public TrainingDataService(ILogger<TrainingDataService> logger, IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _pythonServerUrl = configuration.GetValue<string>("PredictionService:ApiUrl")
            ?? throw new InvalidOperationException("PredictionService:ApiUrl is not configured in appsettings.");
    }

    private async Task SendFileToPythonServer(string filePath, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        using var form = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(filePath);
        var content = new StreamContent(fileStream);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");

        form.Add(content, "file", Path.GetFileName(filePath));

        var response = await client.PostAsync($"{_pythonServerUrl}/upload", form, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Upload failed: {response.StatusCode} - {error}");
        }

        var success = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Upload successful for {FileName}: {Response}", Path.GetFileName(filePath), success);
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

                // === Handle CSV Upload ===
                if (TrainingDataConfigHelper.ShouldUploadCsv())
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var recipeRepository = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
                        var userInteractionRepository = scope.ServiceProvider.GetRequiredService<IUserInteractionRepository>();
                        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                        // Generate recipe-features.csv
                        var trainingRecords = new List<TrainingLogRow>();
                        var recipes = await recipeRepository.GetAllAsync(stoppingToken);
                        foreach (var recipe in recipes)
                        {
                            var detailedRecipe = await recipeRepository.GetRecipeDetailsByIdAsync(recipe.Id, stoppingToken);
                            var logRow = CreateBaseLogRow(recipe);
                            trainingRecords.Add(logRow);
                        }
                        using (var writer = new StreamWriter(TrainingCsvPath, false, Encoding.UTF8))
                        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                        {
                            csv.WriteRecords(trainingRecords);
                        }
                        _logger.LogInformation("Training data CSV written to {CsvPath}", TrainingCsvPath);

                        // Generate interactions.csv
                        var interactionRecords = await userInteractionRepository.GetAggregatedInteractionsAsync();
                        using (var writer = new StreamWriter(InteractionsCsvPath, false, Encoding.UTF8))
                        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                        {
                            csv.WriteRecords(interactionRecords);
                        }
                        _logger.LogInformation("Interactions CSV written to {CsvPath}", InteractionsCsvPath);

                        // Generate user_features.csv
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

                        // Upload all CSVs
                        await SendFileToPythonServer(TrainingCsvPath, stoppingToken);
                        await SendFileToPythonServer(InteractionsCsvPath, stoppingToken);
                        await SendFileToPythonServer(UserFeaturesCsvPath, stoppingToken);
                        TrainingDataConfigHelper.UpdateLastUploadTime();
                        _logger.LogInformation("All CSVs uploaded and timestamp updated.");
                    }
                }
                else
                {
                    _logger.LogInformation("CSV upload skipped (within 6-hour interval).");
                }

                // === Handle Snapshot Upload ===
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

    private TrainingLogRow CreateBaseLogRow(Recipe recipe)
    {
        var nutrition = RecipeNutritionCalculator.CalculateRecipeNutrition(recipe);

        return new TrainingLogRow
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
    private int CalculateAge(DateTime birthDate)
    {
        var today = new DateTime(2025, 6, 16); // Current date as per system
        int age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}



internal class TrainingLogRow
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