using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Interfaces;

public interface IRecipeTagRepository : IRepository<RecipeTag>
{
    Task<RecipeTag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<RecipeTag>> GetByNamesAsync(List<string> names, CancellationToken cancellationToken = default);
}