using LetWeCook.Application.DTOs.Authentications;

namespace LetWeCook.Application.Interfaces;

// wrapper for asp net core identity related operations
public interface IIdentityService
{
    Task<bool> CreateAppUserWithPasswordAsync(string email, string username, string password, Guid siteUserId, CancellationToken cancellationToken = default);
    Task<Guid> CreateAppUserAsync(string email, Guid siteUserId, CancellationToken cancellationToken = default); // Added for external logins
    Task<bool> SignInAsync(string email, string password, CancellationToken cancellationToken = default);
    Task SignInAsync(Guid userId, bool isPersistent, CancellationToken cancellationToken = default); // Added for external logins
    Task<string> GenerateEmailConfirmationTokenAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
    Task<string> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(string email, string token, string password, CancellationToken cancellationToken = default);

    // Updated methods
    Task<Guid?> FindAppUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Guid?> FindAppUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<string> GetUserNameFromAppUserIdAsync(Guid appUserId, CancellationToken cancellationToken = default);
    Task<string> GetUserNameFromSiteUserIdAsync(Guid siteUserId, CancellationToken cancellationToken = default);
    Task<Guid?> GetReferenceSiteUserIdAsync(Guid appUserId, CancellationToken cancellationToken = default);
    Task<bool> AddLoginAsync(Guid userId, ExternalLoginData loginData, CancellationToken cancellationToken = default);
    Task<Guid?> FindUserByLoginAsync(string provider, string providerKey, CancellationToken cancellationToken = default);
}