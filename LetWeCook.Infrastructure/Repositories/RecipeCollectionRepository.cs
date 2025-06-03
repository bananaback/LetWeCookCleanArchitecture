using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Repositories;

public class RecipeCollectionRepository : Repository<RecipeCollection>, IRecipeCollectionRepository
{
    public RecipeCollectionRepository(LetWeCookDbContext context) : base(context)
    {
    }

    public Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken cancellationToken)
    {
        return _dbSet
            .AnyAsync(c => c.CreatedBy.Id == userId && c.Name == name, cancellationToken);
    }

    public async Task<List<RecipeCollection>> GetAllUserCollectionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(c => c.Recipes)
            .ThenInclude(rci => rci.Recipe)
            .ThenInclude(r => r.CoverMediaUrl)
            .Where(c => c.CreatedBy.Id == userId)
            .ToListAsync(cancellationToken);
    }

    public Task<List<string>> GetCollectionNamesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbSet
            .Where(c => c.CreatedBy.Id == userId)
            .Select(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<List<RecipeCollection>> GetRecipeCollectionByNamesAsync(Guid userId, List<string> collectionNames, CancellationToken cancellationToken)
    {
        return _dbSet
            .Include(c => c.Recipes)
            .ThenInclude(rci => rci.Recipe)
            .ThenInclude(r => r.CoverMediaUrl)
            .Where(c => c.CreatedBy.Id == userId && collectionNames.Contains(c.Name))
            .ToListAsync(cancellationToken);
    }

    public Task<RecipeCollection?> GetWithOwnerByIdAsync(Guid collectionId, CancellationToken cancellationToken)
    {
        return _dbSet
            .Include(c => c.CreatedBy)
            .Include(c => c.Recipes)
            .ThenInclude(rci => rci.Recipe)
            .ThenInclude(r => r.CoverMediaUrl)
            .FirstOrDefaultAsync(c => c.Id == collectionId, cancellationToken);
    }
}