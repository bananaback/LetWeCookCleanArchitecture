using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Entities;


public class RecipeCollectionItem : Entity
{
    public Guid RecipeId { get; private set; }
    public Recipe Recipe { get; private set; } = null!;
    public Guid CollectionId { get; private set; }
    public RecipeCollection Collection { get; private set; } = null!;
    public DateTime AddedAt { get; private set; }

    private RecipeCollectionItem() { }

    public RecipeCollectionItem(Guid recipeId, Guid collectionId)
    {
        RecipeId = recipeId;
        CollectionId = collectionId;
        AddedAt = DateTime.UtcNow;
    }
}