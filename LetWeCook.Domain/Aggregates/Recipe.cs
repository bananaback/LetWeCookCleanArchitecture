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
    public MediaUrl CoverMediaUrl { get; private set; } = null!;
    public SiteUser CreatedBy { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public float AverageRating { get; private set; } = 0.0F;
    public int TotalRatings { get; private set; } = 0;
    public int TotalViews { get; private set; } = 0;

    public bool IsVisible { get; private set; }
    public bool IsPreview { get; private set; }
    public List<RecipeTag> Tags { get; private set; } = new List<RecipeTag>();

    public List<RecipeIngredient> RecipeIngredients { get; private set; } = new List<RecipeIngredient>();
    public List<RecipeDetail> RecipeDetails { get; private set; } = new List<RecipeDetail>();

    public List<Donation> Donations { get; private set; } = new List<Donation>();

    private Recipe() { } // for EF Core

    public Recipe(
        string name,
        string description,
        int servings,
        int prepareTime,
        int cookTime,
        DifficultyLevel difficultyLevel,
        MealCategory mealCategory,
        MediaUrl coverMediaUrl,
        SiteUser createdBy,
        bool isVisible,
        bool isPreview)
    {
        Name = name;
        Description = description;
        Servings = servings;
        PrepareTime = prepareTime;
        CookTime = cookTime;
        DifficultyLevel = difficultyLevel;
        MealCategory = mealCategory;
        CoverMediaUrl = coverMediaUrl;
        CreatedBy = createdBy;
        IsVisible = isVisible;
        IsPreview = isPreview;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

    }

    public void AddIngredient(RecipeIngredient recipeIngredient)
    {
        RecipeIngredients.Add(recipeIngredient);
    }

    public void AddIngredients(IEnumerable<RecipeIngredient> recipeIngredients)
    {
        RecipeIngredients.AddRange(recipeIngredients);
    }

    public void AddStep(RecipeDetail recipeDetail)
    {
        RecipeDetails.Add(recipeDetail);
    }

    public void AddSteps(IEnumerable<RecipeDetail> recipeDetails)
    {
        RecipeDetails.AddRange(recipeDetails);
    }

    public void AddRecipeTags(IEnumerable<RecipeTag> recipeTags)
    {
        Tags.AddRange(recipeTags);
    }

    public void SetVisible(bool isVisible)
    {
        IsVisible = isVisible;
    }
    public void SetPreview(bool isPreview)
    {
        IsPreview = isPreview;
    }
}