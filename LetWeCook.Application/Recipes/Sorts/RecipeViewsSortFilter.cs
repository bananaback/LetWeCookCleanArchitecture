using LetWeCook.Application.Enums;
using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Sorts;

public class RecipeViewsSortFilter : ISortFilter<Recipe>
{
    private readonly SortDirection _sortDirection;

    public RecipeViewsSortFilter(SortDirection sortDirection)
    {
        _sortDirection = sortDirection;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        return _sortDirection == SortDirection.Asc
            ? query.OrderBy(r => r.TotalViews)
            : query.OrderByDescending(r => r.TotalViews);
    }
}