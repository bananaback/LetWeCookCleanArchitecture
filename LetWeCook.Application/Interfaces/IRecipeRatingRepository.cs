using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Interfaces;

public interface IRecipeRatingRepository : IRepository<RecipeRating>
{
    Task<RecipeRating?> GetByUserIdAndRecipeIdAsync(Guid userId, Guid recipeId, CancellationToken cancellationToken = default);
    Task<List<RecipeRating>> GetRecipeRatingsAsync(Guid recipeId, CancellationToken cancellationToken = default);
    IQueryable<RecipeRating> GetRecipeRatingsQueryableAsync(Guid recipeId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(IQueryable<RecipeRating> query, CancellationToken cancellationToken = default);
    Task<List<RecipeRating>> QueryableToListAsync(IQueryable<RecipeRating> query, CancellationToken cancellationToken = default);

}