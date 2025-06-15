using LetWeCook.Domain.Exceptions;

namespace LetWeCook.Application.Exceptions;

public class IngredientUpdateException : DomainException
{
    public IngredientUpdateException(string message, ErrorCode errorCode) : base(message, errorCode)
    {
    }

    public IngredientUpdateException(string message, Exception innerException, ErrorCode errorCode) : base(message, innerException, errorCode)
    {
    }
}