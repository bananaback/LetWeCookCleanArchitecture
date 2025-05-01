namespace LetWeCook.Application.Exceptions;

public class DonationRetrievalException : Exception
{
    public DonationRetrievalException(string message) : base(message)
    {
    }

    public DonationRetrievalException(string message, Exception innerException) : base(message, innerException)
    {
    }
}