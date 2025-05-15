using LetWeCook.Application.Dtos.RecipeRating;
using LetWeCook.Application.DTOs;

namespace LetWeCook.Application.Interfaces;

public interface IRecipeRatingService
{
    Task<RecipeRatingDto> CreateRecipeRatingAsync(CreateRecipeRatingRequestDto request, CancellationToken cancellationToken = default);
    Task<PaginatedResult<RecipeRatingDto>> GetRecipeRatingsAsync(Guid recipeId, string sortBy, int page, int pageSize, CancellationToken cancellationToken = default);
    Task SeedRecipeRatingsAsync(int amount, CancellationToken cancellationToken = default);
    Task<RecipeRatingDto> GetUserRatingForRecipeAsync(Guid recipeId, Guid siteUserId, CancellationToken cancellationToken = default);
}