namespace LetWeCook.Application.Exceptions;

public class IngredientUpdateException : Exception
{
    public IngredientUpdateException(string message) : base(message)
    {
    }

    public IngredientUpdateException(string message, Exception innerException) : base(message, innerException)
    {
    }
}