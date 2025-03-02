using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Events;

public class UserLoggedInEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }

    public UserLoggedInEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}