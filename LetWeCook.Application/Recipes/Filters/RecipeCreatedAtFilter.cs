using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Filters;

public class RecipeCreatedAtFilter : IQueryFilter<Recipe>
{
    public DateTime? StartDate { get; }
    public DateTime? EndDate { get; }

    public RecipeCreatedAtFilter(DateTime? startDate, DateTime? endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be greater than end date.");

        StartDate = startDate;
        EndDate = endDate;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        if (StartDate.HasValue)
            query = query.Where(r => r.CreatedAt >= StartDate.Value);

        if (EndDate.HasValue)
            query = query.Where(r => r.CreatedAt <= EndDate.Value);

        return query;
    }
}