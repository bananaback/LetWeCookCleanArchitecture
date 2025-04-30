using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence;

namespace LetWeCook.Infrastructure.Repositories;

public class RecipeTagRepository : Repository<RecipeTag>, IRecipeTagRepository
{
    public RecipeTagRepository(LetWeCookDbContext dbContext) : base(dbContext)
    {
    }
}