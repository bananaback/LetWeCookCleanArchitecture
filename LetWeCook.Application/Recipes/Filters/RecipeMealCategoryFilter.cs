using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Enums;

namespace LetWeCook.Application.Recipes.Filters;

public class RecipeMealCategoryFilter : IQueryFilter<Recipe>
{
    private readonly List<MealCategory> _mealCategories;
    public RecipeMealCategoryFilter(List<MealCategory> mealCategories)
    {
        _mealCategories = mealCategories;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        // If the list of meal categories is empty or null, skip the filter
        if (_mealCategories == null || !_mealCategories.Any())
            return query;

        return query.Where(r => _mealCategories.Contains(r.MealCategory));
    }

}