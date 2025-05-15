using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Events;

public class UserSeededEvent : DomainEvent
{
    public string Email { get; }
    public string Password { get; }
    public string Username { get; }
    public bool IsAdmin { get; }

    public UserSeededEvent(string email, string password, string username, bool isAdmin)
    {
        Email = email;
        Password = password;
        Username = username;
        IsAdmin = isAdmin;
    }
}