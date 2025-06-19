using LetWeCook.Application.DTOs;
using LetWeCook.Application.DTOs.Ingredient;

namespace LetWeCook.Application.Interfaces;

public interface IIngredientService
{
    Task<Guid> AcceptUpdateIngredientAsync(Guid ingredientId, Guid siteUserId, CreateIngredientRequestDto request, CancellationToken cancellationToken);

    Task<Guid> AcceptIngredientAsync(Guid appUserId, CreateIngredientRequestDto request, CancellationToken cancellationToken);
    Task<List<IngredientCategoryDTO>> GetAllIngredientCategoriesAsync(CancellationToken cancellationToken);
    Task<Guid?> CreateIngredientAsync(Guid appUserId, CreateIngredientRequestDto request, CancellationToken cancellationToken);
    Task<Guid?> CreateIngredientForSeedAsync(Guid appUserId, CreateIngredientRequestDto request, CancellationToken cancellationToken);

    Task<IngredientDto> GetIngredientAsync(Guid id, CancellationToken cancellationToken);
    Task<List<IngredientDto>> GetUserIngredientsAsync(Guid siteUserId, CancellationToken cancellationToken);
    Task<List<IngredientDto>> GetRandomIngredientsAsync(int count, CancellationToken cancellationToken);
    Task<List<IngredientDto>> GetIngredientsByCategoryAsync(string category, int count, CancellationToken cancellationToken);
    Task<List<IngredientDto>> GetIngredientsOverviewAsync(CancellationToken cancellationToken);
    Task<IngredientDto> GetIngredientOverviewByIdAsync(Guid ingredientId, Guid siteUserId, bool bypassOwnershipCheck, CancellationToken cancellationToken);
    Task<IngredientDto> GetIngredientPreviewAsync(Guid ingredientId, Guid siteUserId, bool bypassOwnershipCheck, CancellationToken cancellationToken);
    Task<IngredientDto> GetEditingIngredientAsync(Guid ingredientId, Guid siteUserId, CancellationToken cancellationToken);
    Task<Guid?> UpdateIngredientAsync(Guid ingredientId, Guid siteUserId, CreateIngredientRequestDto request, CancellationToken cancellationToken);
}