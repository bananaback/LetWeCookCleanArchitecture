using LetWeCook.Application.Dtos.RecipeCollections;

namespace LetWeCook.Application.DTOs.RecipeCollections;

public class RecipeCollectionDto
{
    public Guid CollectionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<RecipeCollectionItemDto> Items { get; set; } = new List<RecipeCollectionItemDto>();
}