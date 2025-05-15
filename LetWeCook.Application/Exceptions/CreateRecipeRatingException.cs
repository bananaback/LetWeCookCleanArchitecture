namespace LetWeCook.Application.Exceptions;

public class CreateRecipeRatingException : Exception
{
    public CreateRecipeRatingException(string message) : base(message)
    {
    }

    public CreateRecipeRatingException(string message, Exception innerException) : base(message, innerException)
    {
    }
}