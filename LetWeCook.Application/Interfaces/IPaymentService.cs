namespace LetWeCook.Application.Interfaces;

public interface IPaymentService
{
    Task<string> CreateDonationOrderAsync(
            Guid donationId,
            decimal amount,
            string currency,
            string description,
            string payeeEmail);

    Task<(bool Success, string TransactionId, string CustomId, string Error)> CaptureDonationAsync(string orderId);
}