using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Repositories;

public class RecipeRatingRepository : Repository<RecipeRating>, IRecipeRatingRepository
{
    public RecipeRatingRepository(LetWeCookDbContext context) : base(context)
    {
    }

    public Task<int> CountAsync(IQueryable<RecipeRating> query, CancellationToken cancellationToken = default)
    {
        return query.CountAsync(cancellationToken);
    }

    public async Task<RecipeRating?> GetByUserIdAndRecipeIdAsync(Guid userId, Guid recipeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
        .Include(r => r.User)
        .ThenInclude(u => u.Profile)
            .Where(r => r.UserId == userId && r.RecipeId == recipeId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<RecipeRating>> GetRecipeRatingsAsync(Guid recipeId, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(r => r.User)
            .ThenInclude(u => u.Profile)
            .Where(r => r.RecipeId == recipeId)
            .ToListAsync(cancellationToken);
    }

    public IQueryable<RecipeRating> GetRecipeRatingsQueryableAsync(Guid recipeId, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Where(r => r.RecipeId == recipeId)
            .Include(r => r.User)
            .ThenInclude(u => u.Profile)
            .AsQueryable();
    }

    public Task<List<RecipeRating>> QueryableToListAsync(IQueryable<RecipeRating> query, CancellationToken cancellationToken = default)
    {
        return query
            .ToListAsync(cancellationToken);
    }
}