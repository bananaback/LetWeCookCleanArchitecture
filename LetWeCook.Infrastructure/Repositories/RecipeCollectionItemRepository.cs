using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Repositories;

public class RecipeCollectionItemRepository : Repository<RecipeCollectionItem>, IRecipeCollectionItemRepository
{
    public RecipeCollectionItemRepository(LetWeCookDbContext context) : base(context)
    {
    }

    public Task<List<RecipeCollectionItem>> GetItemsByNamesAsync(Guid collectionId, List<string> names, CancellationToken cancellationToken = default)
    {
        return _dbSet.Where(item => item.CollectionId == collectionId && names.Contains(item.Recipe.Name))
            .Include(item => item.Recipe)
            .ThenInclude(r => r.CoverMediaUrl)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<string>> GetRecipeNamesAsync(Guid collectionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(item => item.CollectionId == collectionId)
            .Select(item => item.Recipe.Name)
            .ToListAsync(cancellationToken);
    }
}