using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Repositories;

public class RecipeTagRepository : Repository<RecipeTag>, IRecipeTagRepository
{
    public RecipeTagRepository(LetWeCookDbContext dbContext) : base(dbContext)
    {
    }

    public Task<RecipeTag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _dbSet.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public Task<List<RecipeTag>> GetByNamesAsync(List<string> names, CancellationToken cancellationToken = default)
    {
        return _dbSet.Where(x => names.Contains(x.Name)).ToListAsync(cancellationToken);
    }
}