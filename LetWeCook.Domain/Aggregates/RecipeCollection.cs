using LetWeCook.Domain.Common;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Domain.Aggregates;

public class RecipeCollection : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public int RecipeCount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public SiteUser CreatedBy { get; private set; } = null!;
    public List<RecipeCollectionItem> Recipes { get; private set; } = new List<RecipeCollectionItem>();

    private RecipeCollection() { }

    public RecipeCollection(string name, SiteUser createdBy) : base(Guid.NewGuid())
    {
        Name = name;
        RecipeCount = 0;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void AddRecipe(Recipe recipe)
    {
        if (Recipes.Any(r => r.RecipeId == recipe.Id))
        {
            // Recipe already exists in the collection, no need to add again
            return;
        }

        Recipes.Add(new RecipeCollectionItem(recipe.Id, this.Id));
        RecipeCount++;
    }

    public void RemoveRecipe(Recipe recipe)
    {
        var item = Recipes.FirstOrDefault(r => r.RecipeId == recipe.Id);
        if (item != null)
        {
            Recipes.Remove(item);
            RecipeCount--;
        }
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Collection name cannot be empty.", nameof(newName));
        }

        Name = newName;
    }
}