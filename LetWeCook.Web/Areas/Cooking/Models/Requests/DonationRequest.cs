namespace LetWeCook.Web.Areas.Cooking.Models.Requests;
public class DonationRequest
{
    public Guid RecipeId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string DonationMessage { get; set; } = string.Empty;
}
