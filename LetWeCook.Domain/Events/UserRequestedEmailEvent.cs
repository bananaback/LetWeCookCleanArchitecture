using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Events;

public class UserRequestedEmailEvent : DomainEvent
{
    public string Email { get; }
    public Guid? UserId { get; }

    public UserRequestedEmailEvent(Guid? userId, string email)
    {
        if (userId == null)
            throw new ArgumentNullException(nameof(userId));
        UserId = userId;
        Email = email;
    }
}