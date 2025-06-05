using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Repositories;

public class RecipeRepository : Repository<Recipe>, IRecipeRepository
{
    public RecipeRepository(LetWeCookDbContext context) : base(context)
    {
    }

    public Task<bool> CheckExistByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .AsNoTracking()
            .AnyAsync(r => r.Name == name, cancellationToken);
    }

    public Task<int> CountAsync(IQueryable<Recipe> query, CancellationToken cancellationToken = default)
    {
        return query.CountAsync(cancellationToken);
    }

    public Task<List<Recipe>> GetAllRecipesWithIngredientsAsync(CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Tags)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Recipe>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.CoverMediaUrl)
            .Include(r => r.CreatedBy)
            .Where(r => ids.Contains(r.Id))
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public Task<Recipe?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(r => r.CoverMediaUrl)
            .Include(r => r.CreatedBy)
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public Task<string?> GetNameByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Where(r => r.Id == id)
            .Select(r => r.Name)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Recipe?> GetOverviewByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(r => r.CoverMediaUrl)
            .Include(r => r.CreatedBy)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public IQueryable<Recipe> GetQueryable(CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(r => r.CoverMediaUrl)
            .Where(r => r.IsVisible)
            .AsSplitQuery()
            .AsQueryable();
    }

    public Task<List<Recipe>> GetRandomRecipesAsync(int count = 5, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(r => r.CoverMediaUrl)
            .Where(r => r.IsVisible)
            .OrderBy(r => Guid.NewGuid()) // Random order
            .Take(count)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public Task<Recipe?> GetRecipeDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ThenInclude(i => i.CoverImageUrl)
            .Include(r => r.RecipeDetails)
                .ThenInclude(rd => rd.Detail)
                .ThenInclude(rd => rd.MediaUrls)
            .Include(r => r.Tags)
            .Include(r => r.CoverMediaUrl)
            .Include(r => r.CreatedBy)
                .ThenInclude(cb => cb.Profile)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public Task<Recipe?> GetRecipeWithAuthorByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(r => r.CreatedBy)
                .ThenInclude(cb => cb.Profile)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public Task<List<Recipe>> QueryableToListAsync(IQueryable<Recipe> query, CancellationToken cancellationToken = default)
    {
        return query.ToListAsync(cancellationToken);
    }
}