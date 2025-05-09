using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Filters;

public class RecipeViewsFilter : IQueryFilter<Recipe>
{
    public int MinViews { get; }
    public int MaxViews { get; }

    public RecipeViewsFilter(int minViews, int maxViews)
    {
        if (minViews > maxViews)
            throw new ArgumentException("Min views cannot be greater than max views.");

        MinViews = minViews;
        MaxViews = maxViews;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        if (MinViews > 0)
            query = query.Where(r => r.TotalViews >= MinViews);

        if (MaxViews > 0)
            query = query.Where(r => r.TotalViews <= MaxViews);

        return query;
    }
}