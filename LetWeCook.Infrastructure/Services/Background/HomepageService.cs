using System.Text.Json;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LetWeCook.Infrastructure.Services.Background;

public class HomepageService : BackgroundService
{

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HomepageService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(6);

    public HomepageService(IServiceProvider serviceProvider, ILogger<HomepageService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<LetWeCookDbContext>();

                var topRatedRecipes = await db.Recipes
                    .Include(r => r.CoverMediaUrl)
                    .Include(r => r.CreatedBy)
                        .ThenInclude(u => u.Profile)
                    .Where(r => r.AverageRating > 0 && r.IsVisible == true)
                    .OrderByDescending(r => r.AverageRating)
                    .Take(3)
                    .ToListAsync(stoppingToken);

                var trendingRecipes = await db.Recipes
                    .Include(r => r.CoverMediaUrl)
                    .Include(r => r.CreatedBy)
                        .ThenInclude(u => u.Profile)
                    .Where(r => r.IsVisible == true)
                    .OrderByDescending(r => r.TotalViews)
                    .Take(3)
                    .ToListAsync(stoppingToken);

                var latestRecipes = await db.Recipes
                    .Include(r => r.CoverMediaUrl)
                    .Include(r => r.CreatedBy)
                        .ThenInclude(u => u.Profile)
                    .Where(r => r.IsVisible == true)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(3)
                    .ToListAsync(stoppingToken);

                var latestReviews = await db.RecipeRatings
                    .Include(r => r.Recipe)
                    .Include(r => r.Recipe.CoverMediaUrl)
                    .Include(r => r.User)
                        .ThenInclude(u => u.Profile)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(3)
                    .ToListAsync(stoppingToken);

                var latestDonations = await db.Donations
                    .Include(d => d.Recipe)
                        .ThenInclude(r => r.CoverMediaUrl)
                    .Include(d => d.Donator)
                        .ThenInclude(u => u.Profile)
                    .OrderByDescending(d => d.CreatedAt)
                    .Take(3)
                    .ToListAsync(stoppingToken);

                var homepageData = new HomepageDataDto
                {
                    TopRatedRecipes = topRatedRecipes.Select(r => new RecipeSummaryDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        Servings = r.Servings,
                        PrepareTime = r.PrepareTime,
                        CookTime = r.CookTime,
                        TotalTime = r.TotalTime,
                        DifficultyLevel = r.DifficultyLevel.ToString(),
                        MealCategory = r.MealCategory.ToString(),
                        CoverMediaUrl = r.CoverMediaUrl?.Url.ToString() ?? string.Empty,
                        CreatedBy = r.CreatedBy?.Profile?.Name?.FullName ?? string.Empty,
                        CreatedByProfilePic = r.CreatedBy?.Profile?.ProfilePic ?? string.Empty,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        AverageRating = r.AverageRating,
                        TotalRatings = r.TotalRatings,
                        TotalViews = r.TotalViews
                    }).ToList(),

                    TrendingRecipes = trendingRecipes.Select(r => new RecipeSummaryDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        Servings = r.Servings,
                        PrepareTime = r.PrepareTime,
                        CookTime = r.CookTime,
                        TotalTime = r.TotalTime,
                        DifficultyLevel = r.DifficultyLevel.ToString(),
                        MealCategory = r.MealCategory.ToString(),
                        CoverMediaUrl = r.CoverMediaUrl?.Url.ToString() ?? string.Empty,
                        CreatedBy = r.CreatedBy?.Profile?.Name?.FullName ?? string.Empty,
                        CreatedByProfilePic = r.CreatedBy?.Profile?.ProfilePic ?? string.Empty,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        AverageRating = r.AverageRating,
                        TotalRatings = r.TotalRatings,
                        TotalViews = r.TotalViews
                    }).ToList(),

                    LatestRecipes = latestRecipes.Select(r => new RecipeSummaryDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        Servings = r.Servings,
                        PrepareTime = r.PrepareTime,
                        CookTime = r.CookTime,
                        TotalTime = r.TotalTime,
                        DifficultyLevel = r.DifficultyLevel.ToString(),
                        MealCategory = r.MealCategory.ToString(),
                        CoverMediaUrl = r.CoverMediaUrl?.Url.ToString() ?? string.Empty,
                        CreatedBy = r.CreatedBy?.Profile?.Name?.FullName ?? string.Empty,
                        CreatedByProfilePic = r.CreatedBy?.Profile?.ProfilePic ?? string.Empty,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        AverageRating = r.AverageRating,
                        TotalRatings = r.TotalRatings,
                        TotalViews = r.TotalViews
                    }).ToList(),

                    LatestReviews = latestReviews.Select(r => new ReviewSummaryDto
                    {
                        UserName = r.User?.Profile?.Name?.FullName ?? string.Empty,
                        UserProfilePic = r.User?.Profile?.ProfilePic ?? string.Empty,
                        RecipeName = r.Recipe.Name,
                        RecipeId = r.Recipe.Id,
                        RecipeCoverMediaUrl = r.Recipe.CoverMediaUrl?.Url.ToString() ?? string.Empty,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    }).ToList(),

                    LatestDonations = latestDonations.Select(d => new DonationSummaryDto
                    {
                        Id = d.Id,
                        RecipeId = d.Recipe.Id,
                        RecipeName = d.Recipe.Name,
                        RecipeCoverMediaUrl = d.Recipe.CoverMediaUrl?.Url.ToString() ?? string.Empty,
                        DonatorName = d.Donator?.Profile?.Name?.FullName ?? string.Empty,
                        DonatorProfilePic = d.Donator?.Profile?.ProfilePic ?? string.Empty,
                        DonateMessage = d.DonateMessage,
                        Amount = d.Amount,
                        CreatedAt = d.CreatedAt
                    }).ToList()
                };

                // TODO: Save to cache or expose to API
                // Serialize the DTO to JSON
                var json = JsonSerializer.Serialize(homepageData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // Ensure the directory exists
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data");
                if (!Directory.Exists(webRootPath))
                {
                    Directory.CreateDirectory(webRootPath);
                }

                // Write to wwwroot/data/homepage.json
                var filePath = Path.Combine(webRootPath, "homepage.json");
                await File.WriteAllTextAsync(filePath, json, stoppingToken);

                _logger.LogInformation("✅ Homepage data saved to {Path} at {Time}", filePath, DateTime.UtcNow);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to collect homepage data.");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}

public class RecipeSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Servings { get; set; }
    public int PrepareTime { get; set; }
    public int CookTime { get; set; }
    public int TotalTime { get; set; }
    public string DifficultyLevel { get; set; } = string.Empty;
    public string MealCategory { get; set; } = string.Empty;
    public string CoverMediaUrl { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string CreatedByProfilePic { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public int TotalViews { get; set; }
}

public class ReviewSummaryDto
{
    public string UserName { get; set; } = string.Empty;
    public string UserProfilePic { get; set; } = string.Empty;
    public string RecipeName { get; set; } = string.Empty;
    public Guid RecipeId { get; set; }
    public string RecipeCoverMediaUrl { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class DonationSummaryDto
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public string RecipeCoverMediaUrl { get; set; } = string.Empty;
    public string DonatorName { get; set; } = string.Empty;
    public string DonateMessage { get; set; } = string.Empty;
    public string DonatorProfilePic { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class HomepageDataDto
{
    public IEnumerable<RecipeSummaryDto> TopRatedRecipes { get; set; } = new List<RecipeSummaryDto>();
    public IEnumerable<RecipeSummaryDto> TrendingRecipes { get; set; } = new List<RecipeSummaryDto>();
    public IEnumerable<RecipeSummaryDto> LatestRecipes { get; set; } = new List<RecipeSummaryDto>();
    public IEnumerable<ReviewSummaryDto> LatestReviews { get; set; } = new List<ReviewSummaryDto>();
    public IEnumerable<DonationSummaryDto> LatestDonations { get; set; } = new List<DonationSummaryDto>();
}
