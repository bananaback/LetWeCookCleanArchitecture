namespace LetWeCook.Application.Dtos.RecipeRating;

public class CreateRecipeRatingRequestDto
{
    public Guid RecipeId { get; set; }
    public Guid SiteUserId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;

}