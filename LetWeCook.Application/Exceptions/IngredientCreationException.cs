namespace LetWeCook.Application.Exceptions;

public class IngredientCreationException : Exception
{
    public IngredientCreationException(string message) : base(message)
    {
    }
}