using System.Security.Claims;
using LetWeCook.Application.DTOs.Recipe;
using LetWeCook.Application.Enums;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Application.Recipes;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;
using LetWeCook.Infrastructure.Persistence;
using LetWeCook.Web.Areas.Cooking.Models.Requests;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
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

    [HttpGet("/api/difficulty-enums")]
    public IActionResult GetAllDifficultyEnums()
    {
        var difficultyNames = Enum.GetNames(typeof(DifficultyLevel)).ToList();
        return Json(difficultyNames);
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

    [HttpGet("api/recipes/random")]
    public async Task<IActionResult> GetRandomRecipesAsync(int count, CancellationToken cancellationToken)
    {
        if (count <= 0)
        {
            return BadRequest("Count must be a positive integer.");
        }

        try
        {
            var recipes = await _recipeService.GetRandomRecipesAsync(count, cancellationToken);
            return Ok(recipes);
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

    [HttpGet("api/recipe-details/{id}")]
    public async Task<IActionResult> GetRecipeDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            // Fetching the recipe details (no ownership check, just full public details)
            var recipe = await _recipeService.GetRecipeDetailsAsync(id, cancellationToken);

            // Return the recipe details
            return Ok(recipe);
        }
        catch (RecipeRetrievalException)
        {
            // Handle case where the recipe wasn't found or retrieval failed
            return NotFound(); // 404 Not Found
        }
    }


    public IActionResult Details(Guid id)
    {
        ViewData["RecipeId"] = id;
        return View();
    }

    public IActionResult Browser()
    {
        return View();
    }

    [HttpPost("/api/recipes-browser")]
    public async Task<IActionResult> GetRecipes([FromBody] RecipeQueryRequest request, CancellationToken cancellationToken = default)
    {
        var recipeQueryOptions = new RecipeQueryOptions
        {
            Name = request.NameSearch.Name,
            NameMatchMode = request.NameSearch.TextMatch,
            Difficulties = request.Difficulties,
            MealCategories = request.Categories,
            MinServings = request.Servings.Min,
            MaxServings = request.Servings.Max,
            MinPrepareTime = request.PrepareTime.Min,
            MaxPrepareTime = request.PrepareTime.Max,
            MinCookTime = request.CookTime.Min,
            MaxCookTime = request.CookTime.Max,
            MinAverageRating = request.Rating.Min,
            MaxAverageRating = request.Rating.Max,
            MinTotalViews = request.Views.Min,
            MaxTotalViews = request.Views.Max,
            CreatedFrom = request.CreatedAt.Min,
            CreatedTo = request.CreatedAt.Max,
            UpdatedFrom = request.UpdatedAt.Min,
            UpdatedTo = request.UpdatedAt.Max,
            CreatedByUsername = request.CreatedByUsername ?? string.Empty,
            Tags = request.Tags,
            SortOptions = request.SortOptions.Select(sortOption => new Application.Recipes.SortOption
            {
                Criteria = sortOption.Criteria,
                Direction = sortOption.Direction
            }).ToList(),
            PageNumber = request.Page,
            PageSize = request.ItemsPerPage
        };
        var recipes = await _recipeService.GetRecipes(recipeQueryOptions, cancellationToken);
        return Ok(recipes);
    }

    [HttpPost("/api/personal-recipes-browser")]
    public async Task<IActionResult> GetPersonalRecipes([FromBody] RecipeQueryRequest request, CancellationToken cancellationToken = default)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);

        var recipeQueryOptions = new RecipeQueryOptions
        {
            Name = request.NameSearch.Name,
            NameMatchMode = request.NameSearch.TextMatch,
            Difficulties = request.Difficulties,
            MealCategories = request.Categories,
            MinServings = request.Servings.Min,
            MaxServings = request.Servings.Max,
            MinPrepareTime = request.PrepareTime.Min,
            MaxPrepareTime = request.PrepareTime.Max,
            MinCookTime = request.CookTime.Min,
            MaxCookTime = request.CookTime.Max,
            MinAverageRating = request.Rating.Min,
            MaxAverageRating = request.Rating.Max,
            MinTotalViews = request.Views.Min,
            MaxTotalViews = request.Views.Max,
            CreatedFrom = request.CreatedAt.Min,
            CreatedTo = request.CreatedAt.Max,
            UpdatedFrom = request.UpdatedAt.Min,
            UpdatedTo = request.UpdatedAt.Max,
            CreatedByUsername = request.CreatedByUsername ?? string.Empty,
            Tags = request.Tags,
            SortOptions = request.SortOptions.Select(sortOption => new Application.Recipes.SortOption
            {
                Criteria = sortOption.Criteria,
                Direction = sortOption.Direction
            }).ToList(),
            PageNumber = request.Page,
            PageSize = request.ItemsPerPage
        };
        var recipes = await _recipeService.GetPersonalRecipes(siteUserId, recipeQueryOptions, cancellationToken);
        return Ok(recipes);
    }

    public IActionResult Update(Guid id)
    {
        ViewData["RecipeId"] = id;
        return View();
    }

    [HttpPut("/api/recipes/{id}")]
    public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] CreateRecipeRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return BadRequest("Request body is required.");
        }

        var siteUserId = await GetSiteUserId(cancellationToken);

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

        var updateRecipeRequestId = await _recipeService.UpdateRecipeAsync(id, siteUserId, recipeDto, cancellationToken);

        // Return a success response
        return Ok(updateRecipeRequestId);
    }

    [HttpDelete("/api/recipes/{id}")]
    public async Task<IActionResult> DeleteRecipe(Guid id, CancellationToken cancellationToken = default)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);
        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _recipeService.DeleteRecipeAsync(id, siteUserId, bypassOwnershipCheck: isAdmin, cancellationToken);
            return Ok();
        }
        catch (RecipeRetrievalException)
        {
            return Forbid(); // 403 Forbidden
        }
    }
}