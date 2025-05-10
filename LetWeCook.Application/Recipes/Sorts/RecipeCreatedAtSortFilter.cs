using LetWeCook.Application.Enums;
using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Sorts;

public class RecipeCreatedAtSortFilter : ISortFilter<Recipe>
{
    public SortDirection Direction { get; }

    public RecipeCreatedAtSortFilter(SortDirection direction)
    {
        Direction = direction;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        return Direction == SortDirection.Asc
            ? query.OrderBy(r => r.CreatedAt)
            : query.OrderByDescending(r => r.CreatedAt);
    }
}