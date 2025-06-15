namespace LetWeCook.Application.DTOs.UserInteractions;

public class AggregatedInteractionDto
{
    public Guid UserId { get; set; }
    public Guid RecipeId { get; set; }
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
    public int ViewsCount { get; set; }

    public float Rating { get; set; }             // default 0
    public float CommentLength { get; set; }      // default 0
    public float DonatedAmount { get; set; }      // default 0
}
