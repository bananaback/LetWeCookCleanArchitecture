using LetWeCook.Domain.Exceptions;

namespace LetWeCook.Application.Exceptions;

public class IngredientRetrievalException : DomainException
{
    public IngredientRetrievalException(string message, ErrorCode errorCode) : base(message, errorCode)
    {
    }
}