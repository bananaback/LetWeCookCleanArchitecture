using LetWeCook.Application.DTOs.Donation;

namespace LetWeCook.Application.Interfaces;

public interface IDonationService
{
    Task<Guid> CreateDonationAsync(
        Guid siteUserId,
        Guid recipeId,
        decimal amount,
        string currency,
        string donateMessage,
        CancellationToken cancellationToken = default);
    Task SeedRecipeDonationsAsync(int amount, CancellationToken cancellationToken = default);

    Task<(bool Success, string TransactionId, string CustomId, string Error)> CaptureDonationAsync(string orderId, CancellationToken cancellationToken = default);

    Task<DonationDetailDto> GetDonationDetailsAsync(Guid donationId, CancellationToken cancellationToken);
    Task<List<DonationDetailDto>> GetCompletedDonationsByRecipeIdAsync(Guid recipeId, CancellationToken cancellationToken = default);
}