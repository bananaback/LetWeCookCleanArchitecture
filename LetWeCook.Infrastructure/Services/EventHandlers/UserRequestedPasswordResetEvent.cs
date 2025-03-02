using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Events;
using Microsoft.Extensions.Logging;

namespace LetWeCook.Infrastructure.Services.EventHandlers;

public class UserRequestedPasswordResetEventHandler : INonBlockingDomainEventHandler<UserRequestedPasswordResetEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<UserRequestedPasswordResetEventHandler> _logger;

    public UserRequestedPasswordResetEventHandler(
        IEmailService emailService,
        ILogger<UserRequestedPasswordResetEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }
    public async Task HandleAsync(UserRequestedPasswordResetEvent domainEvent, CancellationToken cancellationToken)
    {
        try
        {
            var resetUrl = $"https://localhost:7212/Identity/Account/ResetPassword?email={Uri.EscapeDataString(domainEvent.Email)}&token={Uri.EscapeDataString(domainEvent.Token)}";
            await _emailService.SendEmailAsync(
                domainEvent.Email,
                "Reset Your Password",
                $"Please reset your password by clicking <a href='{resetUrl}'>here</a>.",
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", domainEvent.Email);
            // Optionally implement retry logic here
        }
    }
}