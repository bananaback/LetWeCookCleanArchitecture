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

    public Task<Recipe?> GetRecipeDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ThenInclude(i => i.CoverImageUrl)
            .Include(r => r.RecipeDetails)
                .ThenInclude(rd => rd.Detail)
            .Include(r => r.Tags)
            .Include(r => r.CoverMediaUrl)
            .Include(r => r.CreatedBy)
                .ThenInclude(cb => cb.Profile)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }
}