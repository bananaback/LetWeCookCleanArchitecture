using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Entities;

public class SuggestionFeedback : Entity
{
    public Guid UserId { get; set; }
    public Guid RecipeId { get; set; }
    public SiteUser User { get; set; } = null!;
    public Recipe Recipe { get; set; } = null!;
    public bool LikeOrDislike { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    private SuggestionFeedback() { } // EF Core requires a parameterless constructor

    public SuggestionFeedback(Guid userId, Guid recipeId, bool likeOrDislike) : base(Guid.NewGuid())
    {
        UserId = userId;
        RecipeId = recipeId;
        LikeOrDislike = likeOrDislike;
        CreatedAt = DateTime.UtcNow;
    }

}