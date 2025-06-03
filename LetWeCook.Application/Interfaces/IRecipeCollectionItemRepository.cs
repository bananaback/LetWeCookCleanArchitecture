using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Interfaces;

public interface IRecipeCollectionItemRepository : IRepository<RecipeCollectionItem>
{
    Task<List<string>> GetRecipeNamesAsync(
        Guid collectionId,
        CancellationToken cancellationToken = default
    );

    Task<List<RecipeCollectionItem>> GetItemsByNamesAsync(
        Guid collectionId,
        List<string> names,
        CancellationToken cancellationToken = default
    );
}