using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Entities;

public class DietaryPreference : Entity
{
    public string Name { get; private set; } = string.Empty;

    private DietaryPreference() : base() { } // For EF Core

    // Constructor for app usage (auto-generates Guid)
    public DietaryPreference(string name) : base(Guid.NewGuid())
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    // Constructor for seeding with explicit Guid
    public DietaryPreference(Guid id, string name) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}