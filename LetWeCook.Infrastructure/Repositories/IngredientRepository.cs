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
            .ToListAsync(cancellationToken);
    }

    public Task<List<Ingredient>> GetAllUserIngreidientOverviewsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbSet.Include(i => i.Category)
            .Include(i => i.CoverImageUrl)
            .Where(i => i.CreatedByUser.Id == userId)
            .ToListAsync(cancellationToken);
    }

    public Task<Ingredient?> GetIngredientByIdWithCategoryAndDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbSet
            .Include(i => i.CreatedByUser)
            .Include(i => i.Category)
            .Include(i => i.CoverImageUrl)
            .Include(i => i.Details)
            .ThenInclude(d => d.MediaUrls)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public Task<List<Ingredient>> GetIngredientOverviewsByCategoryNameAsync(string categoryName, int count, CancellationToken cancellationToken)
    {

        return _dbSet
            .Include(i => i.Category)
            .Include(i => i.CoverImageUrl)
            .Where(i => i.Category.Name == categoryName)
            .OrderBy(_ => Guid.NewGuid()) // Randomize the order
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Ingredient>> GetRandomIngredientOverviewsAsync(int count, CancellationToken cancellationToken)
    {
        return _dbSet
            .Include(i => i.Category)
            .Include(i => i.CoverImageUrl)
            .OrderBy(_ => Guid.NewGuid()) // Randomize the order
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}