using LetWeCook.Application.Enums;
using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Sorts;

public class RecipePrepareTimeSortFilter : ISortFilter<Recipe>
{
    private readonly SortDirection _sortDirection;

    public RecipePrepareTimeSortFilter(SortDirection sortDirection)
    {
        _sortDirection = sortDirection;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        return _sortDirection == SortDirection.Asc
            ? query.OrderBy(r => r.PrepareTime)
            : query.OrderByDescending(r => r.PrepareTime);
    }
}