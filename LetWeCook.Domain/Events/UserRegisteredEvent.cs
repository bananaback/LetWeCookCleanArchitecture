using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Events;

public class UserRegisteredEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public bool Verify { get; }

    public UserRegisteredEvent(Guid userId, string email, bool verify)
    {
        UserId = userId;
        Email = email;
        Verify = verify;
    }
}