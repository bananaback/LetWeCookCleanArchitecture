using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Filters;

public class RecipeUpdatedAtFilter : IQueryFilter<Recipe>
{
    public DateTime? From { get; }
    public DateTime? To { get; }

    public RecipeUpdatedAtFilter(DateTime? from, DateTime? to)
    {
        From = from;
        To = to;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        if (From.HasValue)
            query = query.Where(r => r.UpdatedAt >= From.Value);

        if (To.HasValue)
            query = query.Where(r => r.UpdatedAt <= To.Value);

        return query;
    }
}