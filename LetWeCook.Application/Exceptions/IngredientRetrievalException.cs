namespace LetWeCook.Application.Exceptions;

public class IngredientRetrievalException : Exception
{
    public IngredientRetrievalException(string message) : base(message)
    {
    }
}