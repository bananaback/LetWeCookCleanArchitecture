using LetWeCook.Domain.Exceptions;

namespace LetWeCook.Application.Exceptions;

public class DonationRetrievalException : DomainException
{
    public DonationRetrievalException(string message, ErrorCode errorCode) : base(message, errorCode)
    {
    }

    public DonationRetrievalException(string message, Exception innerException, ErrorCode errorCode) : base(message, innerException, errorCode)
    {
    }
}