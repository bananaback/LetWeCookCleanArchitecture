using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Filters;

public class RecipeCookTimeFilter : IQueryFilter<Recipe>
{
    public int MinCookTime { get; }
    public int MaxCookTime { get; }

    public RecipeCookTimeFilter(int minCookTime, int maxCookTime)
    {
        if (minCookTime > maxCookTime)
            throw new ArgumentException("Min cook time cannot be greater than max cook time.");

        MinCookTime = minCookTime;
        MaxCookTime = maxCookTime;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        if (MinCookTime > 0)
            query = query.Where(r => r.CookTime >= MinCookTime);

        if (MaxCookTime > 0)
            query = query.Where(r => r.CookTime <= MaxCookTime);

        return query;
    }
}