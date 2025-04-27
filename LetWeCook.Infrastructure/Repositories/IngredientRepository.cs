using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Repositories;

public class IngredientRepository : Repository<Ingredient>, IIngredientRepository
{
    public IngredientRepository(LetWeCookDbContext dbContext) : base(dbContext)
    {
    }

    public Task<bool> CheckExistByNameAsync(string name, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(i => i.Name == name, cancellationToken);
    }

    public Task<List<Ingredient>> GetAllIngredientOverviewsAsync(CancellationToken cancellationToken)
    {
        return _dbSet.Include(i => i.Category)
            .Include(i => i.CoverImageUrl)
            .Where(i => i.Visible && !i.IsPreview)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Ingredient>> GetAllUserIngreidientOverviewsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbSet.Include(i => i.Category)
            .Include(i => i.CoverImageUrl)
            .Where(i => i.CreatedByUser.Id == userId && i.Visible && !i.IsPreview)
            .ToListAsync(cancellationToken);
    }

    public Task<Ingredient?> GetIngredientByIdWithCategoryAndDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbSet
            .Include(i => i.CreatedByUser)
            .Include(i => i.Category)
            .Include(i => i.CoverImageUrl)
            .Include(i => i.IngredientDetails)
                .ThenInclude(id => id.Detail)
            .ThenInclude(d => d.MediaUrls)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public Task<Ingredient?> GetIngredientOverviewByIdAsync(Guid ingredientId, CancellationToken cancellationToken)
    {
        return _dbSet.Include(i => i.Category)
            .Include(i => i.CreatedByUser)
            .Include(i => i.CoverImageUrl)
            .Where(i => i.Id == ingredientId)
            .FirstOrDefaultAsync(i => i.Id == ingredientId, cancellationToken);
    }

    public Task<List<Ingredient>> GetIngredientOverviewsByCategoryNameAsync(string categoryName, int count, CancellationToken cancellationToken)
    {

        return _dbSet
            .Include(i => i.Category)
            .Include(i => i.CoverImageUrl)
            .Where(i => i.Category.Name == categoryName && i.Visible && !i.IsPreview)
            .OrderBy(_ => Guid.NewGuid()) // Randomize the order
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Ingredient>> GetRandomIngredientOverviewsAsync(int count, CancellationToken cancellationToken)
    {
        return _dbSet
            .Include(i => i.Category)
            .Include(i => i.CoverImageUrl)
            .Where(i => i.Visible && !i.IsPreview)
            .OrderBy(_ => Guid.NewGuid()) // Randomize the order
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}