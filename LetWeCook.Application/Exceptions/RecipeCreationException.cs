namespace LetWeCook.Application.Exceptions;

public class RecipeCreationException : Exception
{
    public RecipeCreationException(string message) : base(message)
    {
    }

    public RecipeCreationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}