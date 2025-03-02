using LetWeCook.Application.Interfaces;
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

    public async Task<bool> CreateUserAsync(string email, string username, string password, Guid siteUserId, CancellationToken cancellationToken = default)
    {
        var appUser = new ApplicationUser
        {
            UserName = username,
            Email = email,
            SiteUserId = siteUserId,
            EmailConfirmed = false // Require confirmation
        };
        var result = await _userManager.CreateAsync(appUser, password);
        if (result.Succeeded)
        {
            appUser.SyncFromSiteUser(appUser.SiteUser);
            return true;
        }
        return false;
    }

    public async Task<bool> CreateUserAsync(string email, string password, Guid siteUserId, CancellationToken cancellationToken = default)
    {
        return await CreateUserAsync(email, email, password, siteUserId, cancellationToken);
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
}