using System.Text.Json;
using LetWeCook.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaypalServerSdk.Standard;
using PaypalServerSdk.Standard.Authentication;
using PaypalServerSdk.Standard.Controllers;
using PaypalServerSdk.Standard.Exceptions;
using PaypalServerSdk.Standard.Http.Response;
using PaypalServerSdk.Standard.Models;

namespace LetWeCook.Infrastructure.Services;

public class PayPalPaymentService : IPaymentService
{
    private readonly PaypalServerSdkClient _payPalClient;
    private readonly ILogger<PayPalPaymentService> _logger;
    public PayPalPaymentService(
        Microsoft.Extensions.Configuration.IConfiguration configuration,
        ILogger<PayPalPaymentService> logger)
    {
        _logger = logger;
        var clientId = configuration["PayPal:ClientId"]
                ?? throw new ArgumentNullException("PayPal:ClientId is missing.");
        var clientSecret = configuration["PayPal:Secret"]
            ?? throw new ArgumentNullException("PayPal:Secret is missing.");
        _payPalClient = new PaypalServerSdkClient.Builder()
            .ClientCredentialsAuth(
                new ClientCredentialsAuthModel.Builder(
                    clientId,
                    clientSecret
                )
                .Build())
            .Environment(PaypalServerSdk.Standard.Environment.Sandbox)
            .LoggingConfig(config => config
                .LogLevel(LogLevel.Information)
                .RequestConfig(reqConfig => reqConfig.Body(true))
                .ResponseConfig(respConfig => respConfig.Headers(true))
            )
            .Build();
    }

    public async Task<(bool Success, string TransactionId, string CustomId, string Error)> CaptureDonationAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            _logger.LogWarning("CaptureDonationAsync: Invalid or missing orderId.");
            return (false, null, null, "Order ID is required.");
        }

        var captureOrderInput = new CaptureOrderInput
        {
            Id = orderId,
            Prefer = "return=minimal"
        };

        try
        {
            ApiResponse<Order> result = await _payPalClient.OrdersController.CaptureOrderAsync(captureOrderInput);

            if (result == null || result.Data == null)
            {
                _logger.LogError("CaptureDonationAsync: Null response from PayPal.");
                return (false, null, null, "Failed to capture order: No response.");
            }

            // Extract capture object
            var capture = result.Data.PurchaseUnits?.FirstOrDefault()?
                                    .Payments?.Captures?.FirstOrDefault();

            var transactionId = capture?.Id;
            var customId = capture?.CustomId;

            _logger.LogInformation("CaptureDonationAsync: PayPal response: {Response}", JsonSerializer.Serialize(result.Data));

            if (string.IsNullOrWhiteSpace(transactionId))
            {
                _logger.LogError("CaptureDonationAsync: Capture succeeded but transaction ID not found.");
                return (false, null, null, "Capture succeeded but transaction ID not found.");
            }

            return (true, transactionId, customId, null);
        }
        catch (ApiException e)
        {
            _logger.LogError(e, "CaptureDonationAsync: PayPal API exception.");
            return (false, null, null, $"PayPal API error: {e.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CaptureDonationAsync: Unexpected error.");
            return (false, null, null, $"Unexpected error: {ex.Message}");
        }
    }



    public async Task<string> CreateDonationOrderAsync(Guid recipeId, decimal amount, string currency, string description, string payeeEmail, string returnUrl, string cancelUrl)
    {
        // log all the parameters to check if it passed correctly
        _logger.LogInformation("Creating PayPal order with parameters: RecipeId: {RecipeId}, Amount: {Amount}, Currency: {Currency}, Description: {Description}, PayeeEmail: {PayeeEmail}, ReturnUrl: {ReturnUrl}, CancelUrl: {CancelUrl}",
            recipeId, amount, currency, description, payeeEmail, returnUrl, cancelUrl);
        // Validate inputs
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
        {
            throw new ArgumentException("Currency code must be a 3-character ISO-4217 code.");
        }
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive.");
        }
        try
        {
            // Create OrderRequest
            var orderRequest = new OrderRequest
            {
                Intent = CheckoutPaymentIntent.Capture,
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        Amount = new AmountWithBreakdown
                        {
                            CurrencyCode = currency,
                            MValue = amount.ToString("F2"),
                            Breakdown = new AmountBreakdown
                            {
                                ItemTotal = new Money
                                {
                                    CurrencyCode = currency,
                                    MValue = amount.ToString("F2")
                                }
                            }
                        },
                        Description = description,
                        Payee = new PayeeBase { EmailAddress = payeeEmail },
                        CustomId = recipeId.ToString()
                    }
                },
                PaymentSource = new PaymentSource
                {
                    Paypal = new PaypalWallet
                    {
                        ExperienceContext = new PaypalWalletExperienceContext
                        {
                            BrandName = "LetWeCook",
                            ShippingPreference = PaypalWalletContextShippingPreference.NoShipping,
                            UserAction = PaypalExperienceUserAction.Continue,
                            ReturnUrl = returnUrl,
                            CancelUrl = cancelUrl
                        }
                    }
                }
            };

            // Create CreateOrderInput
            var createOrderInput = new CreateOrderInput
            {
                Body = orderRequest,
                Prefer = "return=minimal", // Optimize response
                PaypalRequestId = Guid.NewGuid().ToString() // Unique request ID
            };

            OrdersController ordersController = _payPalClient.OrdersController;

            try
            {
                var response = await ordersController.CreateOrderAsync(createOrderInput);
                var result = response.Data; // Order object

                _logger.LogInformation("PayPal response: {Response}", JsonSerializer.Serialize(result));

                // Find payer-action URL
                var approvalLink = result.Links?.FirstOrDefault(l => l.Rel?.Equals("payer-action", StringComparison.OrdinalIgnoreCase) == true);
                if (approvalLink == null || string.IsNullOrEmpty(approvalLink.Href))
                {
                    throw new InvalidOperationException($"Failed to retrieve PayPal approval URL. Response: {System.Text.Json.JsonSerializer.Serialize(result)}");
                }

                return approvalLink.Href;
            }
            catch (ApiException e)
            {
                // TODO: Handle exception here
                Console.WriteLine(e.Message);
                throw new InvalidOperationException($"PayPal API error: {e.Message}", e);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error creating PayPal order: {ex.Message}", ex);
        }
    }
}