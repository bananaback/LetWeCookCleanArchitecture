using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Filters;

public class RecipePrepareTimeFilter : IQueryFilter<Recipe>
{
    public int MinPrepareTime { get; }
    public int MaxPrepareTime { get; }

    public RecipePrepareTimeFilter(int minPrepareTime, int maxPrepareTime)
    {
        if (minPrepareTime > maxPrepareTime)
            throw new ArgumentException("Min prepare time cannot be greater than max prepare time.");

        MinPrepareTime = minPrepareTime;
        MaxPrepareTime = maxPrepareTime;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        if (MinPrepareTime > 0)
            query = query.Where(r => r.PrepareTime >= MinPrepareTime);

        if (MaxPrepareTime > 0)
            query = query.Where(r => r.PrepareTime <= MaxPrepareTime);

        return query;
    }
}