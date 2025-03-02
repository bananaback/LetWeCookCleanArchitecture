using LetWeCook.Application.DTOs.Authentications;

public interface IHttpContextService
{
    Task ChallengeAsync(string provider, string returnUrl);
    Task<ExternalLoginData> GetExternalLoginInfoAsync();
}