using LetWeCook.Domain.Exceptions;

namespace LetWeCook.Application.Exceptions;

public class IngredientCreationException : DomainException
{
    public IngredientCreationException(string message, ErrorCode errorCode) : base(message, errorCode)
    {
    }
}