using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Entities;

public class Donation : Entity
{
    public Guid RecipeId { get; set; }
    public Guid DonatorId { get; set; }
    public Guid AuthorId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string DonateMessage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ApprovalUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Recipe Recipe { get; set; } = null!;
    public SiteUser Donator { get; set; } = null!;
    public SiteUser Author { get; set; } = null!;

    private Donation() { } // For EF Core

    public Donation(Guid recipeId, Guid donatorId, Guid authorId, decimal amount, string currency, string donateMessage, string status, string approvalUrl) : base(Guid.NewGuid())
    {
        RecipeId = recipeId;
        DonatorId = donatorId;
        AuthorId = authorId;
        Amount = amount;
        Currency = currency;
        DonateMessage = donateMessage;
        Status = status;
        ApprovalUrl = approvalUrl;
    }

    public void SetApprovalUrl(string approvalUrl)
    {
        ApprovalUrl = approvalUrl;
    }
}