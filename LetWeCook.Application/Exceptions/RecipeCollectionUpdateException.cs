using LetWeCook.Domain.Exceptions;

namespace LetWeCook.Application.Exceptions;

public class RecipeCollectionUpdateException : DomainException
{
    public RecipeCollectionUpdateException(string message, ErrorCode errorCode) : base(message, errorCode)
    {
    }

    public RecipeCollectionUpdateException(string message, Exception innerException, ErrorCode errorCode) : base(message, innerException, errorCode)
    {
    }
}