namespace LetWeCook.Application.DTOs.Authentications;

public class ExternalLoginData
{
    public string Provider { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
}
