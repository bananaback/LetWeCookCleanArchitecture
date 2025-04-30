using System.Security.Claims;
using LetWeCook.Application.DTOs.Recipe;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Enums;
using LetWeCook.Infrastructure.Persistence;
using LetWeCook.Web.Areas.Cooking.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Areas.Cooking.Controllers;

[Area("Cooking")]
public class RecipeController : Controller
{
    private readonly IRecipeService _recipeService;
    private readonly UserManager<ApplicationUser> _userManager;
    public RecipeController(
        IRecipeService recipeService,
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _recipeService = recipeService;
    }
    public IActionResult Editor()
    {
        return View();
    }

    [HttpGet("/api/unit-enums")]
    public IActionResult GetAllUnitEnums()
    {
        var unitNames = Enum.GetNames(typeof(UnitEnum)).ToList();
        return Json(unitNames);
    }

    [HttpGet("/api/meal-category-enums")]
    public IActionResult GetAllMealCategoryEnums()
    {
        var mealCategoryNames = Enum.GetNames(typeof(MealCategory)).ToList();
        return Json(mealCategoryNames);
    }

    [HttpGet("/api/recipe-tags")]
    public async Task<IActionResult> GetAllRecipeTags()
    {
        var recipeTags = await _recipeService.GetAllRecipeTagsAsync();
        return Json(recipeTags);
    }

    // ✅ Add recipe creation endpoint
    [HttpPost("/api/recipes")]
    public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest("Request body is required.");
        }

        // Get Authenticated user ID as GUID
        var appUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(appUserIdClaim) || !Guid.TryParse(appUserIdClaim, out var appUserId))
        {
            return Unauthorized("User is not authenticated or invalid ID format.");
        }


        // Map incoming CreateRecipeRequest to CreateRecipeRequestDto
        var recipeDto = new CreateRecipeRequestDto
        {
            Name = request.Name,
            Description = request.Description,
            Servings = request.Servings,
            PrepareTime = request.PrepareTime,
            CookTime = request.CookTime,
            Difficulty = request.Difficulty,
            MealCategory = request.MealCategory,
            Tags = request.Tags,
            Ingredients = request.Ingredients.Select(ingredient => new CreateIngredientDto
            {
                Id = ingredient.Id,
                Quantity = ingredient.Quantity,
                Unit = ingredient.Unit
            }).ToList(),
            Steps = request.Steps.Select(step => new CreateStepDto
            {
                Title = step.Title,
                Description = step.Description,
                MediaUrls = step.MediaUrls
            }).ToList(),
            CoverImage = request.CoverImage
        };

        var createRecipeRequestId = await _recipeService.CreateRecipeAsync(appUserId, recipeDto, cancellationToken);

        // Return the created recipe ID to the frontend
        return Ok(createRecipeRequestId);
    }

    [HttpGet("api/recipe-overview/{id}")]
    public async Task<IActionResult> GetRecipeOverviewAsync(Guid id, CancellationToken cancellationToken)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);
        var isAdmin = User.IsInRole("Admin");

        try
        {
            var ingredient = await _recipeService.GetRecipeOverviewByIdAsync(id, siteUserId, bypassOwnershipCheck: isAdmin, cancellationToken);
            return Ok(ingredient);
        }
        catch (RecipeRetrievalException)
        {
            return Forbid(); // 403 Forbidden
        }
    }

    private async Task<Guid> GetSiteUserId(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        Console.WriteLine("User ID Claim: " + userIdClaim?.Value);
        if (userIdClaim == null) throw new UnauthorizedAccessException("User ID not found in claims.");
        var appUser = await _userManager.FindByIdAsync(userIdClaim.Value);
        if (appUser == null) throw new UnauthorizedAccessException("User not found in database.");
        return appUser.SiteUserId;
    }

    public IActionResult Preview(Guid id)
    {
        ViewData["RecipeId"] = id;
        return View();
    }

    [HttpGet("api/recipe-preview/{id}")]
    public async Task<IActionResult> GetRecipePreviewAsync(Guid id, CancellationToken cancellationToken)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);
        var isAdmin = User.IsInRole("Admin");

        try
        {
            var recipe = await _recipeService.GetRecipePreviewAsync(id, siteUserId, bypassOwnershipCheck: isAdmin, cancellationToken);
            return Ok(recipe);
        }
        catch (RecipeRetrievalException)
        {
            return Forbid(); // 403 Forbidden
        }
    }
}