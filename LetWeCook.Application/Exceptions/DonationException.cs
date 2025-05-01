namespace LetWeCook.Application.Exceptions;

public class DonationException : Exception
{
    public DonationException(string message) : base(message)
    {
    }

    public DonationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}