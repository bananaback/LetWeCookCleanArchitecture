using LetWeCook.Domain.Exceptions;

namespace LetWeCook.Application.Exceptions;

public class RecipeCollectionRetrievalException : DomainException
{

    public RecipeCollectionRetrievalException(string message, ErrorCode errorCode)
        : base(message, errorCode)
    {
    }

    public RecipeCollectionRetrievalException(string message, Exception innerException, ErrorCode errorCode)
        : base(message, innerException, errorCode)
    {
    }
}