using LetWeCook.Application.DTOs.Authentications;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Enums;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace LetWeCook.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<bool> CreateAppUserWithPasswordAsync(
    string email,
    string username,
    string password,
    Guid siteUserId,
    bool isEmailConfirmed = false,
    CancellationToken cancellationToken = default)
    {
        var appUser = new ApplicationUser
        {
            UserName = username,
            Email = email,
            SiteUserId = siteUserId,
            EmailConfirmed = isEmailConfirmed
        };

        // Create the user
        var result = await _userManager.CreateAsync(appUser, password);
        if (!result.Succeeded)
        {
            return false; // Early return if user creation fails
        }

        // Assign the fixed "User" role using enum
        var roleResult = await _userManager.AddToRoleAsync(appUser, UserRole.User.ToString());
        return roleResult.Succeeded;
    }

    public async Task<Guid> CreateAppUserAsync(
        string email,
        Guid siteUserId,
        CancellationToken cancellationToken = default)
    {
        var appUser = new ApplicationUser
        {
            UserName = email, // Use email as username for external logins
            Email = email,
            SiteUserId = siteUserId,
            EmailConfirmed = true // Skip confirmation for external logins
        };

        // Create the user
        var result = await _userManager.CreateAsync(appUser);
        if (!result.Succeeded)
        {
            throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        // Assign the fixed "User" role using enum
        var roleResult = await _userManager.AddToRoleAsync(appUser, UserRole.User.ToString());
        if (!roleResult.Succeeded)
        {
            throw new Exception($"Failed to assign '{UserRole.User}' role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
        }

        return appUser.Id;
    }

    public async Task<bool> SignInAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !user.EmailConfirmed) // Block login if not confirmed
            return false;

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
        return result.Succeeded;
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new InvalidOperationException("User not found.");
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return false;

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task ResetPasswordAsync(string email, string token, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var result = await _userManager.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded)
            throw new InvalidOperationException("Password reset failed.");
    }

    public async Task<Guid?> FindUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user?.SiteUserId;
    }

    public async Task<bool> AddLoginAsync(Guid userId, ExternalLoginData loginData, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;

        var externalLoginInfo = new ExternalLoginInfo(
            principal: null,
            loginProvider: loginData.Provider,
            providerKey: loginData.ProviderKey,
            displayName: loginData.Provider
        );
        var result = await _userManager.AddLoginAsync(user, externalLoginInfo);
        return result.Succeeded;
    }

    public async Task<Guid?> FindUserByLoginAsync(string provider, string providerKey, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByLoginAsync(provider, providerKey);
        return user?.SiteUserId; // Return SiteUserId or null    }
    }



    public async Task SignInAsync(Guid userId, bool isPersistent, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new InvalidOperationException("User not found for sign-in.");

        await _signInManager.SignInAsync(user, isPersistent);
    }

    public async Task<Guid?> FindAppUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user?.Id;
    }


    public async Task<Guid?> FindAppUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user?.SiteUserId;
    }

    public Task<Guid?> GetReferenceSiteUserIdAsync(Guid appUserId, CancellationToken cancellationToken = default)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == appUserId);
        return Task.FromResult(user?.SiteUserId);
    }

    public Task<string> GetUserNameFromAppUserIdAsync(Guid appUserId, CancellationToken cancellationToken = default)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == appUserId);
        if (user == null)
            return Task.FromResult(string.Empty);
        return Task.FromResult(user.UserName ?? "");
    }

    public Task<string> GetUserNameFromSiteUserIdAsync(Guid siteUserId, CancellationToken cancellationToken = default)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.SiteUserId == siteUserId);
        if (user == null)
            return Task.FromResult(string.Empty);
        return Task.FromResult(user.UserName ?? "");
    }
}