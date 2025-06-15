using LetWeCook.Domain.Exceptions;

namespace LetWeCook.Application.Exceptions;

public class DonationException : DomainException
{
    public DonationException(string message, ErrorCode errorCode) : base(message, errorCode)
    {
    }

    public DonationException(string message, Exception innerException, ErrorCode errorCode) : base(message, innerException, errorCode)
    {
    }
}