namespace LetWeCook.Web.Areas.Cooking.Models.Requests;

public class AddRecipeToCollectionRequest
{
    public bool IsNewCollection { get; set; }
    public Guid RecipeId { get; set; }
    public Guid CollectionId { get; set; }
    public string CollectionName { get; set; } = string.Empty;
}