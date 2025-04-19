namespace LetWeCook.Application.Exceptions;

public class RequestRetrievalException : Exception
{
    public RequestRetrievalException(string message) : base(message)
    {
    }

    public RequestRetrievalException(string message, Exception innerException) : base(message, innerException)
    {
    }
}