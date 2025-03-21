using LetWeCook.Domain.Common;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Domain.Aggregates;

public class Ingredient : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public IngredientCategory Category { get; private set; } = null!;

    public float? Calories { get; private set; }
    public float? Protein { get; private set; }
    public float? Carbohydrates { get; private set; }
    public float? Fat { get; private set; }
    public float? Sugar { get; private set; }
    public float? Fiber { get; private set; }
    public float? Sodium { get; private set; }

    public bool IsVegetarian { get; private set; }
    public bool IsVegan { get; private set; }
    public bool IsGlutenFree { get; private set; }
    public bool IsPescatarian { get; private set; }

    public MediaUrl CoverImageUrl { get; private set; } = null!;
    public Guid CoverImageUrlId { get; private set; }
    public float? ExpirationDays { get; private set; }
    public List<Detail> Details { get; private set; } = new List<Detail>();


    public Ingredient() : base() { } // For EF Core

    public Ingredient(
        string name,
        string description,
        Guid categoryId,
        float? calories,
        float? protein,
        float? carbohydrates,
        float? fat,
        float? sugar,
        float? fiber,
        float? sodium,
        bool isVegetarian,
        bool isVegan,
        bool isGlutenFree,
        bool isPescatarian,
        MediaUrl coverImageUrl,
        float? expirationDays,
        List<Detail> details
    )
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        CategoryId = categoryId;
        Calories = calories;
        Protein = protein;
        Carbohydrates = carbohydrates;
        Fat = fat;
        Sugar = sugar;
        Fiber = fiber;
        Sodium = sodium;
        IsVegetarian = isVegetarian;
        IsVegan = isVegan;
        IsGlutenFree = isGlutenFree;
        IsPescatarian = isPescatarian;
        CoverImageUrl = coverImageUrl;
        ExpirationDays = expirationDays;
        Details = details;
    }
}