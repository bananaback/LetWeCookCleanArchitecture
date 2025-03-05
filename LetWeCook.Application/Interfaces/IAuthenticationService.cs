using LetWeCook.Application.DTOs.Authentication;
using LetWeCook.Application.DTOs.Authentications;

namespace LetWeCook.Application.Interfaces;

public interface IAuthenticationService
{
    Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken = default);
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default);
    Task ResendVerificationEmailAsync(string email, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
    Task SendPasswordResetLinkAsync(string email, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(string email, string token, string password, CancellationToken cancellationToken = default);
    Task<bool> SignInWithExternalProviderAsync(string provider, string returnUrl);
    Task<ExternalLoginData?> GetExternalLoginInfoAsync();
    Task<bool> RegisterExternalUserAsync(ExternalLoginData loginData, string email, CancellationToken cancellationToken = default);
}