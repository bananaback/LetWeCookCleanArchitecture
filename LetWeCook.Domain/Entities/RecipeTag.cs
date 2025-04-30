using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Entities;

public class RecipeTag : Entity
{
    public string Name { get; set; } = string.Empty;
    public List<Recipe> Recipes { get; set; } = new List<Recipe>();

    public RecipeTag(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}