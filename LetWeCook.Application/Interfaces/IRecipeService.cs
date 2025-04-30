using LetWeCook.Application.Dtos.Recipe;
using LetWeCook.Application.DTOs.Recipe;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Interfaces;

public interface IRecipeService
{
    Task<List<RecipeTagDto>> GetAllRecipeTagsAsync(CancellationToken cancellationToken = default);
    Task<Guid> CreateRecipeAsync(Guid appUserId, CreateRecipeRequestDto request, CancellationToken cancellationToken = default);
    Task<RecipeDto> GetRecipeOverviewByIdAsync(Guid recipeId, Guid siteUserId, bool bypassOwnershipCheck, CancellationToken cancellationToken);
    Task<RecipeDto> GetRecipePreviewAsync(Guid recipeId, Guid siteUserId, bool bypassOwnershipCheck, CancellationToken cancellationToken);

}