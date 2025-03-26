namespace LetWeCook.Areas.Cooking.Models.Request;

public class CreateIngredientRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public NutritionValues NutritionValues { get; set; } = new();
    public DietaryInfo DietaryInfo { get; set; } = new();
    public string CoverImage { get; set; } = string.Empty;
    public float? ExpirationDays { get; set; } // Added expiration days
    public List<DetailRequest> Details { get; set; } = new();
}

public class NutritionValues
{
    public float? Calories { get; set; }
    public float? Protein { get; set; }
    public float? Carbohydrates { get; set; }
    public float? Fats { get; set; }
    public float? Sugars { get; set; }
    public float? Fiber { get; set; }
    public float? Sodium { get; set; }
}

public class DietaryInfo
{
    public bool IsVegetarian { get; set; }
    public bool IsVegan { get; set; }
    public bool IsGlutenFree { get; set; }
    public bool IsPescatarian { get; set; }
}

public class DetailRequest
{
    public int Order { get; set; } // Ensures correct order tracking
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> MediaUrls { get; set; } = new();
}
