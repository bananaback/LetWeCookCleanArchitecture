using LetWeCook.Domain.Common;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;

namespace LetWeCook.Domain.Aggregates;

public class Recipe : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Servings { get; private set; }
    public int PrepareTime { get; private set; } // in minutes
    public int CookTime { get; private set; } // in minutes
    public int TotalTime => PrepareTime + CookTime; // in minutes
    public DifficultyLevel DifficultyLevel { get; private set; } = DifficultyLevel.UNKNOWN;
    public MealCategory MealCategory { get; private set; } = MealCategory.Unknown;
    public List<RecipeTag> Tags { get; private set; } = new List<RecipeTag>();
    public MediaUrl CoverMediaUrl { get; private set; } = null!;
    public SiteUser CreatedBy { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public float AverageRating { get; private set; } = 0.0F;
    public int TotalRatings { get; private set; } = 0;
    public int TotalViews { get; private set; } = 0;

    public bool IsVisible { get; private set; }
    public bool IsPreview { get; private set; }

    public List<RecipeIngredient> RecipeIngredients { get; private set; } = new List<RecipeIngredient>();
    public List<RecipeDetail> RecipeDetails { get; private set; } = new List<RecipeDetail>();
}