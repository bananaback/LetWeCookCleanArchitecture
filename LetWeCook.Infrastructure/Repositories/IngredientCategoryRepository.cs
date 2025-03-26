using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Repositories;

public class IngredientCategoryRepository : Repository<IngredientCategory>, IIngredientCategoryRepository
{
    public IngredientCategoryRepository(LetWeCookDbContext dbContext) : base(dbContext)
    {
    }

    public Task<IngredientCategory?> GetByNameAsync(string name)
    {
        return _dbSet.FirstOrDefaultAsync(c => c.Name == name);
    }
}