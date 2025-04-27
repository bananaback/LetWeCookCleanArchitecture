using LetWeCook.Domain.Common;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Domain.Aggregates;

public class Ingredient : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public IngredientCategory Category { get; private set; } = null!;

    public Guid CreatedByUserId { get; private set; }
    public SiteUser CreatedByUser { get; private set; } = null!;

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

    public bool Visible { get; private set; }
    public bool IsPreview { get; private set; } = false;

    public MediaUrl CoverImageUrl { get; private set; } = null!;
    public Guid CoverImageUrlId { get; private set; }
    public float? ExpirationDays { get; private set; }
    public List<IngredientDetail> IngredientDetails { get; private set; } = new List<IngredientDetail>();


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
        bool visible,
        bool isPreview,
        MediaUrl coverImageUrl,
        float? expirationDays,
        List<IngredientDetail> details,
        Guid createdByUserId
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
        Visible = visible;
        IsPreview = isPreview;
        CoverImageUrl = coverImageUrl;
        ExpirationDays = expirationDays;
        IngredientDetails = details;
        CreatedByUserId = createdByUserId;
    }

    public void SetVisible(bool visible)
    {
        Visible = visible;
    }

    public void SetPreview(bool isPreview)
    {
        IsPreview = isPreview;
    }

    public void CopyFrom(Ingredient source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        Name = source.Name;
        Description = source.Description;
        CategoryId = source.CategoryId;

        Calories = source.Calories;
        Protein = source.Protein;
        Carbohydrates = source.Carbohydrates;
        Fat = source.Fat;
        Sugar = source.Sugar;
        Fiber = source.Fiber;
        Sodium = source.Sodium;

        IsVegetarian = source.IsVegetarian;
        IsVegan = source.IsVegan;
        IsGlutenFree = source.IsGlutenFree;
        IsPescatarian = source.IsPescatarian;

        Visible = source.Visible;
        IsPreview = source.IsPreview;

        CoverImageUrl.SetUrl(source.CoverImageUrl.Url);
        ExpirationDays = source.ExpirationDays;

    }

    public void AddDetail(IngredientDetail detail)
    {
        if (detail == null) throw new ArgumentNullException(nameof(detail));
        IngredientDetails.Add(detail);
    }

}