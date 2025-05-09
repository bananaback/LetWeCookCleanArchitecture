using System.Linq.Expressions;
using LetWeCook.Application.Enums;
using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Application.Recipes.Filters;

public class RecipeNameFilter : IQueryFilter<Recipe>
{
    public string Name { get; }
    public TextMatchMode MatchMode { get; }

    public RecipeNameFilter(string name, TextMatchMode matchMode = TextMatchMode.Contains)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

        Name = name;
        MatchMode = matchMode;
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        return query.Where(BuildPredicate());
    }

    private Expression<Func<Recipe, bool>> BuildPredicate()
    {
        return MatchMode switch
        {
            TextMatchMode.Exact => r => r.Name == Name,
            TextMatchMode.Contains => r => r.Name != null && r.Name.Contains(Name),
            TextMatchMode.StartsWith => r => r.Name != null && r.Name.StartsWith(Name),
            TextMatchMode.EndsWith => r => r.Name != null && r.Name.EndsWith(Name),
            _ => r => r.Name != null && r.Name.Contains(Name) // default fallback
        };
    }
}
