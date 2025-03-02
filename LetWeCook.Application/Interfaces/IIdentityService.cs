namespace LetWeCook.Application.Interfaces;

public interface IIdentityService
{
    Task<bool> CreateUserAsync(string email, string username, string password, Guid siteUserId, CancellationToken cancellationToken = default);
    Task<bool> CreateUserAsync(string email, string password, Guid siteUserId, CancellationToken cancellationToken = default);
    Task<bool> SignInAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<string> GenerateEmailConfirmationTokenAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
    Task<string> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(string email, string token, string password, CancellationToken cancellationToken = default);
}