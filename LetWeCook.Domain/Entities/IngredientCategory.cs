using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Entities;

public class IngredientCategory : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public IngredientCategory() : base() { } // For EF Core

    public IngredientCategory(string name, string description)
        : this(Guid.NewGuid(), name, description) { }

    public IngredientCategory(Guid id, string name, string description) : base(id)
    {
        Name = name;
        Description = description;
    }
}