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
            ErrorCode.RECIPE_COLLECTION_RECIPE_NOT_FOUND => "Recipe not found in the collection.",
            ErrorCode.RECIPE_COLLECTION_NAME_EMPTY => "Recipe collection name cannot be empty.",
            ErrorCode.RECIPE_COLLECTION_NAME_TOO_LONG => "Recipe collection name is too long.",
            ErrorCode.RECIPE_COLLECTION_NAME_TOO_SHORT => "Recipe collection name is too short.",
            ErrorCode.RECIPE_COLLECTION_NAME_EXISTS => "A recipe collection with this name already exists.",
            ErrorCode.UPDATE_PROFILE_USER_NOT_FOUND => "User not found for profile update.",
            ErrorCode.UPDATE_PROFILE_INVALID_GENDER => "Invalid gender specified.",
            ErrorCode.DONATION_NOT_FOUND => "Donation not found.",
            ErrorCode.DONATION_RECIPE_NOT_FOUND => "Recipe not found for the donation.",
            ErrorCode.DONATION_DONATOR_NOT_FOUND => "Donator not found for the donation.",
            ErrorCode.DONATION_AUTHOR_NOT_FOUND => "Author not found for the donation.",
            ErrorCode.DONATION_AUTHOR_PROFILE_NOT_FOUND => "Author profile not found for the donation.",
            ErrorCode.DONATION_AUTHOR_PAYPAL_NOT_FOUND => "Author's PayPal information not found for the donation.",
            ErrorCode.INGREDIENT_NOT_FOUND => "Ingredient not found.",
            ErrorCode.INGREDIENT_NOT_OWNED_BY_USER => "Ingredient does not belong to the user.",
            ErrorCode.INGREDIENT_USER_NOT_FOUND => "User not found for the ingredient.",
            ErrorCode.INGREDIENT_NAME_EXISTS => "An ingredient with this name already exists.",
            ErrorCode.INGREDIENT_SITE_USER_ID_IS_NULL => "Ingredient site user ID cannot be null.",
            ErrorCode.RECIPE_NOT_FOUND => "Recipe not found.",
            _ => "An unexpected error occurred."
        };
    }
}
