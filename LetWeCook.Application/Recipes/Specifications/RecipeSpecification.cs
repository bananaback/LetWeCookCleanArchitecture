using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Specifications;

public class RecipeSpecification : ISpecification<Recipe>
{
    private readonly List<IQueryFilter<Recipe>> _filters = new();
    private readonly List<ISortFilter<Recipe>> _sorts = new();

    public RecipeSpecification AddFilter(IQueryFilter<Recipe> filter)
    {
        _filters.Add(filter);
        return this;
    }

    public RecipeSpecification AddSort(ISortFilter<Recipe> sort)
    {
        _sorts.Add(sort);
        return this;
    }

    public IQueryable<Recipe> ApplyPagination(IQueryable<Recipe> query, int pageNumber, int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        // Apply fitlers
        foreach (var filter in _filters)
        {
            query = filter.Apply(query);
        }

        // Apply sorts
        foreach (var sort in _sorts)
        {
            query = sort.Apply(query);
        }

        return query;
    }
}