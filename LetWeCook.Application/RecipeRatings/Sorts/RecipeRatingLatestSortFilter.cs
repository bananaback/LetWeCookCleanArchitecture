using LetWeCook.Application.Enums;
using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.RecipeRatings.Sorts;

public class RecipeRatingLatestSortFilter : ISortFilter<RecipeRating>
{
    private readonly SortDirection _sortDirection;

    public RecipeRatingLatestSortFilter(SortDirection sortDirection)
    {
        _sortDirection = sortDirection;
    }

    public IQueryable<RecipeRating> Apply(IQueryable<RecipeRating> query)
    {
        // if updated at is default, use created at
        return _sortDirection == SortDirection.Asc
            ? query.OrderBy(r => r.UpdatedAt == default ? r.CreatedAt : r.UpdatedAt)
            : query.OrderByDescending(r => r.UpdatedAt == default ? r.CreatedAt : r.UpdatedAt);
    }
}