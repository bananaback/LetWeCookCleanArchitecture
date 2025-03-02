namespace LetWeCook.Application.DTOs.Authentication;

public class RegisterResponseDTO
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = "Registration successful!";
}