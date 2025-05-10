using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Interfaces;

public interface IIngredientRepository : IRepository<Ingredient>
{
    Task<Ingredient?> GetIngredientByIdWithCategoryAndDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> CheckExistByNameAsync(string name, CancellationToken cancellationToken);
    Task<List<Ingredient>> GetRandomIngredientOverviewsAsync(int count, CancellationToken cancellationToken);
    Task<Ingredient?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<List<Ingredient>> GetIngredientOverviewsByCategoryNameAsync(string categoryName, int count, CancellationToken cancellationToken);
    Task<List<Ingredient>> GetAllIngredientOverviewsAsync(CancellationToken cancellationToken);
    Task<Ingredient?> GetIngredientOverviewByIdAsync(Guid ingredientId, CancellationToken cancellationToken);
    Task<List<Ingredient>> GetAllUserIngreidientOverviewsAsync(Guid userId, CancellationToken cancellationToken);
}