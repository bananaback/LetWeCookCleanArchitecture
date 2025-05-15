using LetWeCook.Application.Enums;
using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.RecipeRatings.Sorts;

public class RecipeRatingRatingSortFilter : ISortFilter<RecipeRating>
{
    private readonly SortDirection _sortDirection;

    public RecipeRatingRatingSortFilter(SortDirection sortDirection)
    {
        _sortDirection = sortDirection;
    }

    public IQueryable<RecipeRating> Apply(IQueryable<RecipeRating> query)
    {
        return _sortDirection == SortDirection.Asc
            ? query.OrderBy(r => r.Rating)
            : query.OrderByDescending(r => r.Rating);
    }
}