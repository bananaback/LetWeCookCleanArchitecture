namespace LetWeCook.Application.Dtos.Recipe;

public class RecipeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Servings { get; set; }
    public int PrepareTime { get; set; }
    public int CookTime { get; set; }
    public int TotalTime => PrepareTime + CookTime;
    public string Difficulty { get; set; } = string.Empty;
    public string MealCategory { get; set; } = string.Empty;
    public string CoverImage { get; set; } = string.Empty;
    public AuthorProfileDto AuthorProfile { get; set; } = new AuthorProfileDto();
    public DateTime CreatedAt { get; set; }
    public float AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public int TotalViews { get; set; }

    public List<string> Tags { get; set; } = new List<string>();
    public List<RecipeIngredientDto> Ingredients { get; set; } = new List<RecipeIngredientDto>();
    public List<RecipeStepDto> Steps { get; set; } = new List<RecipeStepDto>();
}

public class AuthorProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ProfilePicUrl { get; set; }
    public string? Bio { get; set; }

    // Optional: public social links
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
}

public class RecipeIngredientDto
{
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public float Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class RecipeStepDto
{
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> MediaUrls { get; set; } = new List<string>();
}
