using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Filters;

public class RecipeCreatedByUserIdFilter : IQueryFilter<Recipe>
{
    private readonly Guid _userId;

    public RecipeCreatedByUserIdFilter(Guid userId)
    {
        _userId = userId;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        return query.Where(r => r.CreatedBy.Id == _userId);
    }
}