using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Interfaces;

public interface IIngredientCategoryRepository : IRepository<IngredientCategory>
{
    Task<IngredientCategory?> GetByNameAsync(string name);
}