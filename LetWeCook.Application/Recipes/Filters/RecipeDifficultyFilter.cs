using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Enums;

namespace LetWeCook.Application.Recipes.Filters;

public class RecipeDifficultyFilter : IQueryFilter<Recipe>
{
    private readonly List<DifficultyLevel> _difficultyLevels;
    public RecipeDifficultyFilter(List<DifficultyLevel> difficultyLevels)
    {
        _difficultyLevels = difficultyLevels;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        // If no difficulty levels are specified, do not filter
        if (_difficultyLevels == null || !_difficultyLevels.Any())
            return query;

        return query.Where(r => _difficultyLevels.Contains(r.DifficultyLevel));
    }

}