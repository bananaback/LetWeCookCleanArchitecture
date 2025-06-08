using LetWeCook.Application.Dtos.Recipe;
using LetWeCook.Application.DTOs;
using LetWeCook.Application.DTOs.Recipe;
using LetWeCook.Application.Recipes;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Interfaces;

public interface IRecipeService
{
    Task<List<RecipeTagDto>> GetAllRecipeTagsAsync(CancellationToken cancellationToken = default);
    Task<List<RecipeDto>> GetRandomRecipesAsync(int count, CancellationToken cancellationToken = default);
    Task<Guid> CreateRecipeAsync(Guid appUserId, CreateRecipeRequestDto request, CancellationToken cancellationToken = default);
    Task<Guid> CreateRecipeForSeedAsync(Guid appUserId, CreateRecipeRequestDto request, CancellationToken cancellationToken = default);
    Task<RecipeDto> GetRecipeOverviewByIdAsync(Guid recipeId, Guid siteUserId, bool bypassOwnershipCheck, CancellationToken cancellationToken);
    Task<RecipeDto> GetRecipePreviewAsync(Guid recipeId, Guid siteUserId, bool bypassOwnershipCheck, CancellationToken cancellationToken);
    Task<RecipeDto> GetRecipeDetailsAsync(Guid recipeId, CancellationToken cancellationToken);

    Task<PaginatedResult<RecipeDto>> GetRecipes(RecipeQueryOptions recipeQueryOptions, CancellationToken cancellationToken = default);
    Task<PaginatedResult<RecipeDto>> GetPersonalRecipes(Guid siteUserId, RecipeQueryOptions recipeQueryOptions, CancellationToken cancellationToken = default);
    Task<Guid?> UpdateRecipeAsync(Guid recipeId, Guid siteUserId, CreateRecipeRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteRecipeAsync(Guid recipeId, Guid siteUserId, bool bypassOwnershipCheck, CancellationToken cancellationToken = default);
}