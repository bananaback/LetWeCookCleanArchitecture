using LetWeCook.Application.Specifications;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Recipes.Filters;


public class RecipeTagsFilter : IQueryFilter<Recipe>
{
    private readonly List<Guid> _tagIds; // Assuming the tags are identified by their GUIDs.

    public RecipeTagsFilter(List<RecipeTag> tags)
    {
        _tagIds = tags.Select(tag => tag.Id).ToList(); // Convert tags to their IDs
    }

    public IQueryable<Recipe> Apply(IQueryable<Recipe> query)
    {
        // If the list of tags is empty or null, skip the filter
        if (_tagIds == null || !_tagIds.Any())
            return query;

        return query.Where(r => r.Tags.Any(t => _tagIds.Contains(t.Id))); // Filter by tag IDs
    }
}
