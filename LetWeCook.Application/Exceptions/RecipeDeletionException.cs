namespace LetWeCook.Application.Exceptions;

public class RecipeDeletionException : Exception
{
    public RecipeDeletionException(string message) : base(message)
    {
    }

    public RecipeDeletionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}