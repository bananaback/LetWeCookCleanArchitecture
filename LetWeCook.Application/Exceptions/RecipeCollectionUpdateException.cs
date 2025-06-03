namespace LetWeCook.Application.Exceptions;

public class RecipeCollectionUpdateException : Exception
{
    public RecipeCollectionUpdateException(string message) : base(message)
    {
    }

    public RecipeCollectionUpdateException(string message, Exception innerException) : base(message, innerException)
    {
    }
}