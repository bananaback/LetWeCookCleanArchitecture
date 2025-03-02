namespace LetWeCook.Application.Exceptions;

public class PasswordResetException : Exception
{
    public PasswordResetException(string message) : base(message)
    {
    }
}
