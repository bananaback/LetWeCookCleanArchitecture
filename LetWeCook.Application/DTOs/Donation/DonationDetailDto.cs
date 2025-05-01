
namespace LetWeCook.Application.DTOs.Donation;

public class DonationDetailDto
{
    public Guid DonationId { get; set; }
    public Guid AuthorId { get; set; }

    public Guid RecipeId { get; set; }

    public ProfileDto AuthorProfileDto { get; set; } = new ProfileDto();
    public ProfileDto DonatorProfileDto { get; set; } = new ProfileDto();

    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string DonateMessage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ApprovalUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public RecipeOverviewDto RecipeOverview { get; set; } = new RecipeOverviewDto();
}


public class RecipeOverviewDto
{
    public Guid RecipeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
}

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ProfilePicUrl { get; set; }
    public string? Bio { get; set; }

    // Optional: public social links
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? PayPalEmail { get; set; }
}