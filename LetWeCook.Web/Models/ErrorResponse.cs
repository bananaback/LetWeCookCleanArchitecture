namespace LetWeCook.Web.Models;

public class ErrorResponse
{
    public string ErrorCode { get; init; } = "UnknownError";

    public string Message { get; set; } = "An unexpected error occurred.";

    public string ExceptionMessage { get; init; } = string.Empty;
}
