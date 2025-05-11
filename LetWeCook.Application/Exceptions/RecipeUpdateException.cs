namespace LetWeCook.Application.Exceptions;

public class RecipeUpdateException : Exception
{
    public RecipeUpdateException(string message) : base(message)
    {
    }

    public RecipeUpdateException(string message, Exception innerException) : base(message, innerException)
    {
    }
}