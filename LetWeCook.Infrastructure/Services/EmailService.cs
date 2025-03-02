using LetWeCook.Application.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace LetWeCook.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IEmailSender _identityEmailSender;

    public EmailService(IEmailSender identityEmailSender)
    {
        _identityEmailSender = identityEmailSender ?? throw new ArgumentNullException(nameof(identityEmailSender));
    }

    public async Task SendEmailAsync(string to, string subject, string htmlMessage, CancellationToken cancellationToken = default)
    {
        // Check for cancellation before proceeding
        cancellationToken.ThrowIfCancellationRequested();

        // Call IEmailSender's method (no CancellationToken support)
        await _identityEmailSender.SendEmailAsync(to, subject, htmlMessage);

        // Note: If the underlying IEmailSender implementation doesn't support cancellation,
        // this task will complete once started unless cancelled before the call.
    }
}