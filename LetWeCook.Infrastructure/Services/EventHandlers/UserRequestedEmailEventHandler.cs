using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Events;
using Microsoft.Extensions.Logging;

namespace LetWeCook.Infrastructure.Services.EventHandlers;

public class UserRequestedEmailHandler : INonBlockingDomainEventHandler<UserRequestedEmailEvent>
{
    private readonly IEmailService _emailService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<UserRegisteredEmailHandler> _logger;

    public UserRequestedEmailHandler(
        IEmailService emailService,
        IIdentityService identityService,
        ILogger<UserRegisteredEmailHandler> logger)
    {
        _emailService = emailService;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task HandleAsync(UserRequestedEmailEvent domainEvent, CancellationToken cancellationToken)
    {
        try
        {
            // Always generate the token here
            var token = await _identityService.GenerateEmailConfirmationTokenAsync(domainEvent.Email, cancellationToken);

            var verificationUrl = $"/Identity/Account/VerifyEmail?email={Uri.EscapeDataString(domainEvent.Email)}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendEmailAsync(
                            domainEvent.Email,
                            "Verify Your Email",
                            $"Please verify your email by clicking <a href='{verificationUrl}'>here</a>.",
                            cancellationToken);

            _logger.LogInformation("Verification email sent to {Email} for user {UserId}", domainEvent.Email, domainEvent.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email to {Email} for user {UserId}", domainEvent.Email, domainEvent.UserId);
            // Optionally implement retry logic here
        }
    }
}