namespace LetWeCook.Application.Exceptions;

public class UserRegistrationException : ApplicationException
{
    public UserRegistrationException(string message) : base(message) { }
}