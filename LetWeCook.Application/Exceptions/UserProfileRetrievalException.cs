namespace LetWeCook.Application.Exceptions;

public class UserProfileRetrievalException : Exception
{
    public UserProfileRetrievalException(string message) : base(message) { }
}