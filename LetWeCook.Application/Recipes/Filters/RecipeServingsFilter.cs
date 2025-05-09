using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Filters;

public class RecipeServingsFilter : IQueryFilter<Recipe>
{
    public int? MinServings { get; }
    public int? MaxServings { get; }

    public RecipeServingsFilter(int minServings, int maxServings)
    {
        if (minServings > maxServings)
            throw new ArgumentException("Min servings cannot be greater than max servings.");

        MinServings = minServings;
        MaxServings = maxServings;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        if (MinServings is not null)
            query = query.Where(r => r.Servings >= MinServings.Value);

        if (MaxServings is not null)
            query = query.Where(r => r.Servings <= MaxServings.Value);

        return query;
    }
}
