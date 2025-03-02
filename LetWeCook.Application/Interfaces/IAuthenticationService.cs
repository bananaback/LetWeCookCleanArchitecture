using LetWeCook.Application.DTOs.Authentication;

namespace LetWeCook.Application.Interfaces;

public interface IAuthenticationService
{
    Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken = default);
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default);
    Task ResendVerificationEmailAsync(string email, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
    Task SendPasswordResetLinkAsync(string email, CancellationToken cancellationToken = default);
}