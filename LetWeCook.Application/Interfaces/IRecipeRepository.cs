using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Interfaces;

public interface IRecipeRepository : IRepository<Recipe>
{
    Task<Recipe?> GetOverviewByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Recipe?> GetRecipeDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Recipe?> GetRecipeWithAuthorByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string?> GetNameByIdAsync(Guid id, CancellationToken cancellationToken = default);
}