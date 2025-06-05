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

    public async Task<int> CountRecipeAverageCommentLengthAsync(Guid recipeId, CancellationToken cancellationToken = default)
    {
        var lengths = await _dbSet
        .Where(r => r.RecipeId == recipeId && r.Comment != null)
        .Select(r => r.Comment)
        .ToListAsync(cancellationToken);

        if (lengths.Count == 0)
            return 0;

        return (int)lengths.Average(c => c.Length);
    }

    public async Task<RecipeRating?> GetByUserIdAndRecipeIdAsync(Guid userId, Guid recipeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
        .Include(r => r.User)
        .ThenInclude(u => u.Profile)
            .Where(r => r.UserId == userId && r.RecipeId == recipeId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public IQueryable<RecipeRating> GetRecipeAverageCommentLength(Guid recipeId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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