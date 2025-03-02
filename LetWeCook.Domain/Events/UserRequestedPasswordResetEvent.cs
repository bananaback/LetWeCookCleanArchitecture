using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Events;

public class UserRequestedPasswordResetEvent : DomainEvent
{
    public string Email { get; }
    public string Token { get; }

    public UserRequestedPasswordResetEvent(string email, string token)
    {
        Email = email;
        Token = token;
    }
}