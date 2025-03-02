using LetWeCook.Application.DTOs.Authentications;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace LetWeCook.Infrastructure.Services;

public class HttpContextService : IHttpContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public HttpContextService(IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _signInManager = signInManager;
    }

    public async Task ChallengeAsync(string provider, string returnUrl)
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, returnUrl);
        await _httpContextAccessor.HttpContext!.ChallengeAsync(provider, properties);
    }

    public Task<ExternalLoginData> GetExternalLoginInfoAsync()
    {
        throw new NotImplementedException();
    }
}