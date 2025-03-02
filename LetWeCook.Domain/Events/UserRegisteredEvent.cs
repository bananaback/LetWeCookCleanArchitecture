using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Events;

public class UserRegisteredEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string VerificationToken { get; }

    public UserRegisteredEvent(Guid userId, string email, string verificationToken)
    {
        UserId = userId;
        Email = email;
        VerificationToken = verificationToken;
    }
}