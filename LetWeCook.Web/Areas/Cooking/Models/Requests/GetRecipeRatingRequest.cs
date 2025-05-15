namespace LetWeCook.Web.Areas.Cooking.Models.Requests;

public class GetRecipeRatingRequest
{
    public int RecipeId { get; set; }
    public string SortBy { get; set; } = "Newest";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

}