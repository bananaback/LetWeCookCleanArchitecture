namespace LetWeCook.Application.Interfaces;

public interface IPaymentService
{
    Task<string> CreateDonationOrderAsync(
            Guid donationId,
            decimal amount,
            string currency,
            string description,
            string payeeEmail,
            string returnUrl,
            string cancelUrl);

    Task<(bool Success, string TransactionId, string CustomId, string Error)> CaptureDonationAsync(string orderId);
}