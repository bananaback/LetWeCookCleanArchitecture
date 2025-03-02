using LetWeCook.Application.DTOs.Authentication;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Common;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Events;
using LetWeCook.Domain.Exceptions;

namespace LetWeCook.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IHttpContextService _httpContextService;

    public AuthenticationService(
        IUserRepository userRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher domainEventDispatcher,
        IHttpContextService httpContextService)
    {
        _userRepository = userRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _domainEventDispatcher = domainEventDispatcher;
        _httpContextService = httpContextService;
    }

    public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            throw new UserRegistrationException("Email, username, and password are required.");

        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null && !existingUser.IsRemoved)
            throw new UserAlreadyExistsException(request.Email);

        var siteUser = new SiteUser(request.Email); // No token generation here

        try
        {
            await _userRepository.AddAsync(siteUser, cancellationToken);
            var identityCreated = await _identityService.CreateUserAsync(request.Email, request.Username, request.Password, siteUser.Id, cancellationToken);
            if (!identityCreated)
                throw new UserRegistrationException("Failed to register user in identity system.");

            await _unitOfWork.CommitAsync(cancellationToken);

        }
        catch (Exception ex)
        {
            throw new UserRegistrationException($"Registration failed: {ex.Message}");
        }

        return new RegisterResponseDTO
        {
            UserId = siteUser.Id,
            Email = request.Email,
            Message = "Registration successful! Please check your email to verify your account."
        };
    }

    public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            throw new AuthenticationException("Email and password are required.");

        try
        {
            var success = await _identityService.SignInAsync(request.Email, request.Password, cancellationToken);
            if (!success)
                throw new AuthenticationException("Invalid email or password, or email not verified.");

            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user != null)
            {
                user.RecordLogin(request.Email);
                await _unitOfWork.CommitAsync(cancellationToken);
            }

            return new LoginResponseDTO
            {
                Email = request.Email,
                Success = true
            };
        }
        catch (Exception ex)
        {
            throw new AuthenticationException($"Login failed: {ex.Message}");
        }
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _identityService.ConfirmEmailAsync(email, token, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new EmailConfirmationException($"Email verification failed: {ex.Message}");
        }
    }

    public async Task ResendVerificationEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null || user.IsRemoved)
            throw new InvalidOperationException("User not found.");

        var newEvent = new UserRequestedEmailEvent(user.Id, email);
        // Dispatch the event to trigger email sending
        await _domainEventDispatcher.DispatchEventsAsync(new List<DomainEvent> { newEvent }, cancellationToken);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Delegate sign-out to IIdentityService
            await _identityService.SignOutAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new AuthenticationException($"Logout failed: {ex.Message}");
        }
    }

    public async Task SendPasswordResetLinkAsync(string email, CancellationToken cancellationToken = default)
    {
        var token = await _identityService.GeneratePasswordResetTokenAsync(email, cancellationToken);

        var newEvent = new UserRequestedPasswordResetEvent(email, token);
        // Dispatch the event to trigger email sending
        await _domainEventDispatcher.DispatchEventsAsync(new List<DomainEvent> { newEvent }, cancellationToken);
    }

    public async Task ResetPasswordAsync(string email, string token, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            await _identityService.ResetPasswordAsync(email, token, password, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new PasswordResetException($"Password reset failed: {ex.Message}");
        }
    }


    public async Task<bool> SignInWithExternalProviderAsync(string provider, string returnUrl)
    {
        // Pass provider and returnUrl directly; no AuthenticationProperties here
        await _httpContextService.ChallengeAsync(provider, returnUrl);
        return true; // Challenge redirects, so assume success
    }
}