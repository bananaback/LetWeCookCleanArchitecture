using System.Security.Claims;
using LetWeCook.Application.Dtos.RecipeRating;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Areas.Cooking.Models.Requests;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Areas.Cooking.Controllers;

[Area("Cooking")]
public class RecipeRatingController : Controller
{
    private readonly IRecipeRatingService _recipeRatingService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RecipeRatingController(
        IRecipeRatingService recipeRatingService,
        UserManager<ApplicationUser> userManager)
    {
        _recipeRatingService = recipeRatingService;
        _userManager = userManager;
    }

    [HttpPost("api/recipe-ratings")]
    public async Task<IActionResult> CreateRecipeRating([FromBody] CreateRecipeRatingRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var siteUserId = await GetSiteUserId();

        var createRecipeRatingRequestDto = new CreateRecipeRatingRequestDto
        {
            RecipeId = request.RecipeId,
            SiteUserId = siteUserId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        try
        {
            var result = await _recipeRatingService.CreateRecipeRatingAsync(createRecipeRatingRequestDto, cancellationToken);
            return Ok(result);
        }
        catch (CreateRecipeRatingException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "An error occurred while creating the recipe rating: " + ex.Message });
        }

    }

    [HttpGet("api/recipe-ratings/{recipeId}")]
    public async Task<IActionResult> GetRecipeRatings(Guid recipeId, string sortBy = "Newest", int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var ratings = await _recipeRatingService.GetRecipeRatingsAsync(recipeId, sortBy, page, pageSize, cancellationToken);
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "An error occurred while fetching the recipe ratings: " + ex.Message });
        }
    }

    [Authorize]
    [HttpGet("api/recipe-ratings/user/{recipeId}")]
    public async Task<IActionResult> GetUserRatingForRecipe(Guid recipeId)
    {
        try
        {
            var siteUserId = await GetSiteUserId();

            var rating = await _recipeRatingService.GetUserRatingForRecipeAsync(recipeId, siteUserId);
            return Ok(rating);

        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "An error occurred while fetching the user rating: " + ex.Message });
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