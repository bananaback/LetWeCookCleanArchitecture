namespace LetWeCook.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlMessage, CancellationToken cancellationToken = default);
}