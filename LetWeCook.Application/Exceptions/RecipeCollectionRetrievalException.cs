namespace LetWeCook.Application.Exceptions;

public class RecipeCollectionRetrievalException : Exception
{
    public RecipeCollectionRetrievalException()
        : base("An error occurred while retrieving recipe collections.")
    {
    }

    public RecipeCollectionRetrievalException(string message)
        : base(message)
    {
    }

    public RecipeCollectionRetrievalException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}