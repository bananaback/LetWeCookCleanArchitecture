using LetWeCook.Application.DTOs.Authentications;

namespace LetWeCook.Application.Interfaces;

public interface IExternalAuthService
{
    Task<ExternalLoginData?> GetExternalLoginDataAsync();
}