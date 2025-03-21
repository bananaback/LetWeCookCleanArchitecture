using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence;

namespace LetWeCook.Infrastructure.Repositories;

public class IngredientCategoryRepository : Repository<IngredientCategory>, IIngredientCategoryRepository
{
    public IngredientCategoryRepository(LetWeCookDbContext dbContext) : base(dbContext)
    {
    }
}