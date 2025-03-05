namespace LetWeCook.Domain.Exceptions;

public class UserAlreadyExistsException : DomainException
{
    public UserAlreadyExistsException(string email)
        : base($"A user with the email/username '{email}' already exists.") { }
}