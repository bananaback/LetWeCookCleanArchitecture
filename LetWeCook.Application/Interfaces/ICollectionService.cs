using LetWeCook.Application.Dtos.RecipeCollections;
using LetWeCook.Application.DTOs;
using LetWeCook.Application.DTOs.RecipeCollections;

namespace LetWeCook.Application.Interfaces;

public interface ICollectionService
{
    Task<List<RecipeCollectionDto>> GetRecipeCollectionsAsync(Guid siteUserId, CancellationToken cancellationToken);
    Task AddRecipeToCollectionAsync(Guid siteUserId, Guid recipeId, Guid collectionId, CancellationToken cancellationToken);
    Task AddRecipeToNewCollectionAsync(Guid siteUserId, Guid recipeId, string collectionName, CancellationToken cancellationToken);
    Task UpdateCollectionNameAsync(Guid siteUserId, Guid collectionId, string newName, CancellationToken cancellationToken);
    Task DeleteCollectionAsync(Guid siteUserId, Guid collectionId, CancellationToken cancellationToken);
    Task<PaginatedResult<RecipeCollectionDto>> BrowseCollection(
        Guid siteUserId,
        RecipeCollectionQueryRequestDto request,
        CancellationToken cancellationToken = default
    );

    Task<PaginatedResult<RecipeCollectionItemDto>> BrowseCollectionItems(
        Guid siteUserId,
        RecipeCollectionItemQueryRequestDto request,
        CancellationToken cancellationToken = default
    );

    Task RemoveRecipeFromCollectionAsync(
        Guid siteUserId,
        Guid recipeId,
        Guid collectionId,
        CancellationToken cancellationToken
    );
}