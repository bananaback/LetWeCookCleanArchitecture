using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Common;
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
    private const string CsvPath = "training_data.csv";
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

        form.Add(content, "file", "training_data.csv");

        var response = await client.PostAsync($"{_pythonServerUrl}/upload", form, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Upload failed: {response.StatusCode} - {error}");
        }

        var success = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Upload successful: {success}");
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
                        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                        var donationRepository = scope.ServiceProvider.GetRequiredService<IDonationRepository>();
                        var ratingRepository = scope.ServiceProvider.GetRequiredService<IRecipeRatingRepository>();
                        var suggestionFeedbackRepository = scope.ServiceProvider.GetRequiredService<ISuggestionFeedbackRepository>();

                        var donations = await donationRepository.GetAllAsync(stoppingToken);
                        var ratings = await ratingRepository.GetAllAsync(stoppingToken);
                        var suggestionFeedbacks = await suggestionFeedbackRepository.GetAllAsync(stoppingToken);

                        var records = new List<TrainingLogRow>();

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

                            var record = CreateBaseLogRow(user, recipe, rating.CreatedAt);
                            record.Rating = rating.Rating;
                            record.CommentLength = rating.Comment?.Length ?? 0;
                            records.Add(record);
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

                            var record = CreateBaseLogRow(donator, recipe, donation.CreatedAt);
                            record.DonatedAmount = donation.Amount;
                            records.Add(record);
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

                            var record = CreateBaseLogRow(user, feedback.Recipe, feedback.CreatedAt);
                            record.Liked = feedback.LikeOrDislike;
                            records.Add(record);
                        }

                        // Write to CSV
                        using var writer = new StreamWriter(CsvPath, false, Encoding.UTF8);
                        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
                        csv.WriteRecords(records);

                        _logger.LogInformation("Training data CSV written to {CsvPath}", CsvPath);
                    }

                    await SendFileToPythonServer(CsvPath, stoppingToken);
                    TrainingDataConfigHelper.UpdateLastUploadTime();
                    _logger.LogInformation("Training data uploaded and timestamp updated.");
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


    private TrainingLogRow CreateBaseLogRow(SiteUser user, Recipe recipe, DateTime timestamp)
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