namespace LetWeCook.Areas.Cooking.Models.Requests;

public class CreateRecipeRatingRequest
{
    public Guid RecipeId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}