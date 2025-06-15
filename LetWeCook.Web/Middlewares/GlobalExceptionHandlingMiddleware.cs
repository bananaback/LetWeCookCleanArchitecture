using LetWeCook.Domain.Exceptions;
using LetWeCook.Web.Models;

namespace LetWeCook.Web.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Proceed to next middleware
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception occurred.");

        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        if (ex is DomainException appEx)
        {
            response = MapExceptionToErrorResponse(appEx);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            response = new ErrorResponse
            {
                ErrorCode = "InternalServerError",
                Message = "An unexpected error occurred.",
                ExceptionMessage = ex.ToString()
            };
        }

        await context.Response.WriteAsJsonAsync(response);
    }

    private ErrorResponse MapExceptionToErrorResponse(DomainException ex)
    {
        var response = new ErrorResponse
        {
            ErrorCode = ex.ErrorCode.ToString() ?? "DomainException",
            ExceptionMessage = ex.ToString()
        };

        response.Message = GetUserFriendlyMessage(ex.ErrorCode, ex.ContextData) ?? ex.Message;

        return response;
    }

    private string GetUserFriendlyMessage(ErrorCode errorCode, Dictionary<string, string> contextData)
    {
        return errorCode switch
        {
            ErrorCode.UNKNOWN_ERROR => "An unknown error occurred.",
            ErrorCode.USER_ALREADY_EXISTS => "A user with this email already exists.",
            ErrorCode.USER_NOT_FOUND => "User not found.",
            ErrorCode.RECIPE_COLLECTION_USER_NOT_FOUND => "Recipe collection user not found.",
            ErrorCode.RECIPE_COLLECTION_NOT_FOUND => "Recipe collection not found.",
            ErrorCode.RECIPE_COLLECTION_NOT_BELONG_TO_USER => "This recipe collection does not belong to the user.",
            ErrorCode.INGREDIENT_NAME_EXISTS => "An ingredient with this name already exists.",
            _ => "An unexpected error occurred."
        };
    }
}
