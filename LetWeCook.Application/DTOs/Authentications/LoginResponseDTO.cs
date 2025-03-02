namespace LetWeCook.Application.DTOs.Authentication;

public class LoginResponseDTO
{
    public string Email { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
}