namespace LetWeCook.Application.Exceptions;

public class EmailConfirmationException : ApplicationException
{
    public EmailConfirmationException(string message) : base(message) { }
}