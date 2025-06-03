using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Interfaces;

public interface IRecipeCollectionRepository : IRepository<RecipeCollection>
{
    Task<List<RecipeCollection>> GetAllUserCollectionsAsync(Guid userId, CancellationToken cancellationToken);
    Task<RecipeCollection?> GetWithOwnerByIdAsync(Guid collectionId, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken cancellationToken);
    Task<List<string>> GetCollectionNamesAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<RecipeCollection>> GetRecipeCollectionByNamesAsync(
        Guid userId,
        List<string> collectionNames,
        CancellationToken cancellationToken);
}