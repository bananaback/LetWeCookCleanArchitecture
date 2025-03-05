using System.Security.Claims;
using LetWeCook.Application.DTOs.Authentications;
using LetWeCook.Application.Interfaces;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace LetWeCook.Infrastructure.Services;

public class ExternalAuthService : IExternalAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ExternalAuthService(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<ExternalLoginData?> GetExternalLoginDataAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return null;

        string avatarUrl = string.Empty;
        if (info.LoginProvider == "Google")
            avatarUrl = info.Principal.FindFirstValue("picture") ?? string.Empty;
        else if (info.LoginProvider == "Facebook")
            avatarUrl = $"https://graph.facebook.com/{info.ProviderKey}/picture?type=large";

        return new ExternalLoginData
        {
            Provider = info.LoginProvider,
            ProviderKey = info.ProviderKey,
            Email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            AvatarUrl = avatarUrl ?? string.Empty
        };
    }
}