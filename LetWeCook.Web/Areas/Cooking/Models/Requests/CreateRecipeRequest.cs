namespace LetWeCook.Web.Areas.Cooking.Models.Requests;
public class CreateRecipeRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Servings { get; set; }
    public int PrepareTime { get; set; } // in minutes
    public int CookTime { get; set; } // in minutes
    public string Difficulty { get; set; } = string.Empty;
    public string MealCategory { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new List<string>();
    public List<IngredientRequest> Ingredients { get; set; } = new List<IngredientRequest>();
    public List<RecipeStepRequest> Steps { get; set; } = new List<RecipeStepRequest>();
    public string CoverImage { get; set; } = string.Empty;
}

public class IngredientRequest
{
    public Guid Id { get; set; } // Ingredient UUID
    public float Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class RecipeStepRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> MediaUrls { get; set; } = new List<string>();
}
