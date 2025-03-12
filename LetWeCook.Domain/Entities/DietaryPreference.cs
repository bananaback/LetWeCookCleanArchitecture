using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Entities;

public class DietaryPreference : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private DietaryPreference() : base() { } // For EF Core

    // Constructor for app usage (auto-generates Guid)
    public DietaryPreference(string name, string description) : base(Guid.NewGuid())
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    // Constructor for seeding with explicit Guid
    public DietaryPreference(Guid id, string name, string description) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }
}