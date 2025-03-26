namespace LetWeCook.Application.DTOs.Ingredient;

public class CreateIngredientRequestDto
{
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public NutritionValuesRequestDto NutritionValues { get; set; } = new();
    public DietaryInfoRequestDto DietaryInfo { get; set; } = new();
    public string CoverImage { get; set; } = string.Empty;
    public float? ExpirationDays { get; set; }
    public List<DetailRequestDto> Details { get; set; } = new();
}

public class NutritionValuesRequestDto
{
    public float? Calories { get; set; }
    public float? Protein { get; set; }
    public float? Carbohydrates { get; set; }
    public float? Fats { get; set; }
    public float? Sugars { get; set; }
    public float? Fiber { get; set; }
    public float? Sodium { get; set; }
}

public class DietaryInfoRequestDto
{
    public bool IsVegetarian { get; set; }
    public bool IsVegan { get; set; }
    public bool IsGlutenFree { get; set; }
    public bool IsPescatarian { get; set; }
}

public class DetailRequestDto
{
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> MediaUrls { get; set; } = new();
}
