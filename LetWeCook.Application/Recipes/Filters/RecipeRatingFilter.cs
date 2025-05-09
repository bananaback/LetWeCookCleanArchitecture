using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Filters;

public class RecipeRatingFilter : IQueryFilter<Recipe>
{
    public double MinRating { get; }
    public double MaxRating { get; }

    public RecipeRatingFilter(double minRating, double maxRating)
    {
        if (minRating > maxRating)
            throw new ArgumentException("Min rating cannot be greater than max rating.");

        MinRating = minRating;
        MaxRating = maxRating;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        if (MinRating > 0)
            query = query.Where(r => r.AverageRating >= MinRating);

        if (MaxRating > 0)
            query = query.Where(r => r.AverageRating <= MaxRating);

        return query;
    }
}