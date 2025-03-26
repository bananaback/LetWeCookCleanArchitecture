namespace LetWeCook.Application.DTOs.Ingredient;

public class IngredientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty; // Added for better UI rendering

    public float? Calories { get; set; }
    public float? Protein { get; set; }
    public float? Carbohydrates { get; set; }
    public float? Fats { get; set; }
    public float? Sugars { get; set; }
    public float? Fiber { get; set; }
    public float? Sodium { get; set; }

    public bool IsVegetarian { get; set; }
    public bool IsVegan { get; set; }
    public bool IsGlutenFree { get; set; }
    public bool IsPescatarian { get; set; }

    public string CoverImageUrl { get; set; } = string.Empty; // Now stores the actual image URL
    public float? ExpirationDays { get; set; }

    public List<DetailDto> Details { get; set; } = new();
}

public class DetailDto
{
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> MediaUrls { get; set; } = new();
}
