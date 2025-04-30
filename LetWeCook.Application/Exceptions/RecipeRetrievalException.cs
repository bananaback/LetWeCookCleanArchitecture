namespace LetWeCook.Application.Exceptions;

public class RecipeRetrievalException : Exception
{
    public RecipeRetrievalException(string message) : base(message)
    {
    }

    public RecipeRetrievalException(string message, Exception innerException) : base(message, innerException)
    {
    }
}