namespace LetWeCook.Areas.Cooking.Models.Requests;

public class RemoveRecipeFromCollectionRequest
{
    public Guid RecipeId { get; set; }
    public Guid CollectionId { get; set; }
}