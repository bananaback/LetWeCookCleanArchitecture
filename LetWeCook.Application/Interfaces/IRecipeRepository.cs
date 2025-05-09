using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Interfaces;

public interface IRecipeRepository : IRepository<Recipe>
{
    Task<Recipe?> GetOverviewByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Recipe?> GetRecipeDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Recipe?> GetRecipeWithAuthorByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string?> GetNameByIdAsync(Guid id, CancellationToken cancellationToken = default);

    IQueryable<Recipe> GetQueryable(CancellationToken cancellationToken = default);
    Task<int> CountAsync(IQueryable<Recipe> query, CancellationToken cancellationToken = default);
    Task<List<Recipe>> QueryableToListAsync(IQueryable<Recipe> query, CancellationToken cancellationToken = default);
}