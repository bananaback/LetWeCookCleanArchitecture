namespace LetWeCook.Application.DTOs.Recipe;
public class CreateRecipeRequestDto
{
    public string Name { get; set; } = string.Empty;  // Recipe name, default to empty string
    public string Description { get; set; } = string.Empty;  // Recipe description, default to empty string
    public int Servings { get; set; }  // Number of servings
    public int PrepareTime { get; set; }
    public int CookTime { get; set; }
    public string Difficulty { get; set; } = string.Empty;  // Difficulty level, default to empty string
    public string MealCategory { get; set; } = string.Empty;  // Meal category, default to empty string
    public List<string> Tags { get; set; } = new List<string>();  // List of recipe tags, default to empty list
    public List<CreateIngredientDto> Ingredients { get; set; } = new List<CreateIngredientDto>();  // List of ingredients, default to empty list
    public List<CreateStepDto> Steps { get; set; } = new List<CreateStepDto>();  // List of steps, default to empty list
    public string CoverImage { get; set; } = string.Empty;  // URL of the cover image, default to empty string
}

// The CreateIngredientDto and CreateStepDto should be defined elsewhere
public class CreateIngredientDto
{
    public Guid Id { get; set; } = Guid.Empty;  // Ingredient ID, default to empty GUID
    public float Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;  // Unit of the ingredient (e.g., "g", "ml", etc.), default to empty string
}

public class CreateStepDto
{
    public string Title { get; set; } = string.Empty;  // Step title, default to empty string
    public string Description { get; set; } = string.Empty;  // Step description, default to empty string
    public List<string> MediaUrls { get; set; } = new List<string>();  // List of media URLs, default to empty list
    public int Order { get; set; }  // Step order, default to 0 
}
