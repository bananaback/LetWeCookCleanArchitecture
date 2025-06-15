using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Entities;

public class UserInteraction : Entity
{
    public Guid UserId { get; private set; }
    public Guid RecipeId { get; private set; }
    public string InteractionType { get; private set; } = string.Empty;
    public float EventValue { get; private set; } = 0.0f;
    public DateTime InteractionDate { get; private set; } = DateTime.UtcNow;

    public UserInteraction() { } // For EF Core

    public UserInteraction(
        Guid userId,
        Guid recipeId,
        string interactionType,
        float eventValue
    )
    {
        UserId = userId;
        RecipeId = recipeId;
        InteractionType = interactionType;
        EventValue = eventValue;
    }

}