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

    Task<(bool Success, string TransactionId, string Error)> CaptureDonationAsync(string orderId);

    Task<DonationDetailDto> GetDonationDetailsAsync(Guid donationId, CancellationToken cancellationToken);
}