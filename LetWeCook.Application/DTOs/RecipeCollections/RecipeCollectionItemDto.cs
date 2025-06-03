namespace LetWeCook.Application.Dtos.RecipeCollections;

public class RecipeCollectionItemDto
{
    public Guid RecipeId { get; set; }
    public Guid CollectionId { get; set; }
    public string CollectionName { get; set; } = string.Empty;
    public string RecipeName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}