using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Entities;

public class DietaryPreference : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty; // Hex or CSS color
    public string Emoji { get; private set; } = string.Empty; // Unicode emoji

    private DietaryPreference() : base() { } // For EF Core

    // Constructor for app usage (auto-generates Guid)
    public DietaryPreference(string name, string description, string color, string emoji)
        : base(Guid.NewGuid())
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Color = color ?? throw new ArgumentNullException(nameof(color));
        Emoji = emoji ?? throw new ArgumentNullException(nameof(emoji));
    }

    // Constructor for seeding with explicit Guid
    public DietaryPreference(Guid id, string name, string description, string color, string emoji)
        : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Color = color ?? throw new ArgumentNullException(nameof(color));
        Emoji = emoji ?? throw new ArgumentNullException(nameof(emoji));
    }
}
