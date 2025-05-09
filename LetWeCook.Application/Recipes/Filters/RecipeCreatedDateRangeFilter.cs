using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Filters;

public class RecipeCreatedDateRangeFilter : IQueryFilter<Recipe>
{
    public DateTime? StartDate { get; }
    public DateTime? EndDate { get; }

    public RecipeCreatedDateRangeFilter(DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            throw new ArgumentException("StartDate cannot be after EndDate.");

        StartDate = startDate;
        EndDate = endDate;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        if (StartDate is not null)
            query = query.Where(r => r.CreatedAt >= StartDate.Value);

        if (EndDate is not null)
            query = query.Where(r => r.CreatedAt <= EndDate.Value);

        return query;
    }
}
