using System.Security.Claims;
using LetWeCook.Application.Interfaces;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Areas.Cooking.Controllers;

[Area("Cooking")]
public class SuggestionController : Controller
{
    private readonly IRecipeSuggestionService _recipeSuggestionService;
    private readonly UserManager<ApplicationUser> _userManager;

    public SuggestionController(IRecipeSuggestionService recipeSuggestionService, UserManager<ApplicationUser> userManager)
    {
        _recipeSuggestionService = recipeSuggestionService;
        _userManager = userManager;
    }

    [HttpGet("api/suggestions")]
    public async Task<IActionResult> GetRecipeSuggestion(int count = 5, CancellationToken cancellationToken = default)
    {
        try
        {
            Guid? siteUserIdNullable = await GetSiteUserId();

            if (siteUserIdNullable == null)
                throw new Exception("User ID not found");

            Guid siteUserId = siteUserIdNullable.Value;

            var suggestions = await _recipeSuggestionService.GetUserSpecificSuggestionsAsync(siteUserId, count, cancellationToken);
            return Ok(suggestions);
        }
        catch (Exception)
        {
            var suggestions = await _recipeSuggestionService.GetRandomSuggestionsAsync(count, cancellationToken);
            return Ok(suggestions);
        }
    }


    [Authorize]
    [HttpPost("api/suggestions")]
    public async Task<IActionResult> FeedBack(Guid recipeId, bool isLike, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(recipeId.ToString()))
        {
            return BadRequest("Invalid feedback data.");
        }

        try
        {
            var siteUserId = await GetSiteUserId();
            await _recipeSuggestionService.ProcessFeedbackAsync(recipeId, isLike, siteUserId, cancellationToken);
            return Ok("Feedback processed successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task<Guid> GetSiteUserId()
    {
        // Get user id from the authenticated user
        var appUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (appUserId == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        var appUser = await _userManager.FindByIdAsync(appUserId);

        if (appUser == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        return appUser.SiteUserId;
    }
}