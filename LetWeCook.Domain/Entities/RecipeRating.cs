using LetWeCook.Domain.Aggregates;

namespace LetWeCook.Domain.Entities;

public class RecipeRating
{
    public Recipe Recipe { get; set; } = null!;
    public Guid RecipeId { get; set; }
    public SiteUser User { get; set; } = null!;
    public Guid UserId { get; set; }
    public int Rating { get; set; } // Assuming rating is an integer value
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    private RecipeRating() { } // For EF Core

    public RecipeRating(Guid recipeId, Guid siteUserId, int rating, string comment)
    {
        RecipeId = recipeId;
        UserId = siteUserId;
        Rating = rating;
        Comment = comment;
    }
}