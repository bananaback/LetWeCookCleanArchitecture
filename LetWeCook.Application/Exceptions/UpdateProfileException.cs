using LetWeCook.Domain.Exceptions;

namespace LetWeCook.Application.Exceptions;

public class UpdateProfileException : DomainException
{
    public UpdateProfileException(string message, ErrorCode errorCode) : base(message, errorCode)
    {
    }
}