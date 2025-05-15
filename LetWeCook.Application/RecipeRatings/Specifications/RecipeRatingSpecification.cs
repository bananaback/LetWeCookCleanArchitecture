using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.RecipeRatings.Specifications;

public class RecipeRatingSpecification : ISpecification<RecipeRating>
{
    private readonly List<ISortFilter<RecipeRating>> _sorts = new();

    public RecipeRatingSpecification AddSort(ISortFilter<RecipeRating> sort)
    {
        _sorts.Add(sort);
        return this;
    }

    public IQueryable<RecipeRating> Apply(IQueryable<RecipeRating> query)
    {
        // Apply sorts
        foreach (var sort in _sorts)
        {
            query = sort.Apply(query);
        }
        return query;
    }

    public IQueryable<RecipeRating> ApplyPagination(IQueryable<RecipeRating> query, int page, int pageSize)
    {
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

}
