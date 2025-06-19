using LetWeCook.Application.DTOs.Ingredient;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Areas.Cooking.Models.Request;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LetWeCook.Web.Areas.Cooking.Controllers;

[Area("Cooking")]
public class IngredientController : Controller
{
    private readonly IIngredientService _ingredientService;
    private readonly UserManager<ApplicationUser> _userManager;
    public IngredientController(
        IIngredientService ingredientService,
        UserManager<ApplicationUser> userManager)
    {
        _ingredientService = ingredientService;
        _userManager = userManager;

    }

    public IActionResult Browser()
    {
        return View();
    }

    public IActionResult Preview(Guid id)
    {
        ViewData["IngredientId"] = id;
        return View();
    }

    public IActionResult Details(Guid id)
    {
        ViewData["IngredientId"] = id;
        return View();
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Editor()
    {
        return View();
    }

    public IActionResult Update(Guid id)
    {
        ViewData["IngredientId"] = id;
        return View();
    }

    [HttpGet("api/ingredient/{id}/overview")]
    public async Task<IActionResult> GetIngredientOverviewAsync(Guid id, CancellationToken cancellationToken)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);
        var isAdmin = User.IsInRole("Admin");
        var ingredient = await _ingredientService.GetIngredientOverviewByIdAsync(id, siteUserId, bypassOwnershipCheck: isAdmin, cancellationToken);
        return Ok(ingredient);
    }

    [HttpPut("api/ingredient/{id}")]
    public async Task<IActionResult> UpdateIngredientAsync(Guid id, [FromBody] CreateIngredientRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest("Request body is required.");
        }

        var appUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(appUserIdClaim) || !Guid.TryParse(appUserIdClaim, out var appUserId))
        {
            return Unauthorized("User is not authenticated or invalid ID format.");
        }

        var appUser = await _userManager.FindByIdAsync(appUserIdClaim);
        if (appUser == null)
        {
            return Unauthorized("User not found in database.");
        }

        var siteUserId = appUser.SiteUserId;

        var requestDto = new CreateIngredientRequestDto
        {
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            NutritionValues = new NutritionValuesRequestDto
            {
                Calories = request.NutritionValues.Calories,
                Protein = request.NutritionValues.Protein,
                Carbohydrates = request.NutritionValues.Carbohydrates,
                Fats = request.NutritionValues.Fats,
                Sugars = request.NutritionValues.Sugars,
                Fiber = request.NutritionValues.Fiber,
                Sodium = request.NutritionValues.Sodium
            },
            DietaryInfo = new DietaryInfoRequestDto
            {
                IsVegetarian = request.DietaryInfo.IsVegetarian,
                IsVegan = request.DietaryInfo.IsVegan,
                IsGlutenFree = request.DietaryInfo.IsGlutenFree,
                IsPescatarian = request.DietaryInfo.IsPescatarian
            },
            CoverImage = request.CoverImage,
            ExpirationDays = request.ExpirationDays,
            Details = request.Details.Select(detail => new DetailRequestDto
            {
                Order = detail.Order,
                Title = detail.Title,
                Description = detail.Description,
                MediaUrls = detail.MediaUrls
            }).ToList()
        };

        Guid idResult;
        if (request.AcceptImmediately)
        {
            idResult = await _ingredientService.AcceptUpdateIngredientAsync(id, siteUserId, requestDto, cancellationToken);
        }
        else
        {
            idResult = await _ingredientService.UpdateIngredientAsync(id, siteUserId, requestDto, cancellationToken)
                ?? throw new Exception("Failed to create update ingredient request.");
        }

        return Ok(idResult);
    }

    [AllowAnonymous]
    [HttpGet("/api/ingredient-categories")]
    public async Task<IActionResult> GetAllIngredientCategoriesAsync()
    {
        var ingredientCategories = await _ingredientService.GetAllIngredientCategoriesAsync(CancellationToken.None);
        return Ok(ingredientCategories);
    }

    // Create get ingredient by guid endpoint
    [AllowAnonymous]
    [HttpGet("api/ingredient/{id}")]
    public async Task<IActionResult> GetIngredientAsync(Guid id, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientService.GetIngredientAsync(id, cancellationToken);
        return Ok(ingredient);
    }

    [HttpGet("api/ingredient/{id}/editing")]
    public async Task<IActionResult> GetEditingIngredientAsync(Guid id, CancellationToken cancellationToken)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);
        var ingredient = await _ingredientService.GetEditingIngredientAsync(id, siteUserId, cancellationToken);
        return Ok(ingredient);
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

    [HttpGet("api/ingredient/{id}/preview")]
    public async Task<IActionResult> GetIngredientPreviewAsync(Guid id, CancellationToken cancellationToken)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);
        var isAdmin = User.IsInRole("Admin");

        var ingredient = await _ingredientService.GetIngredientPreviewAsync(id, siteUserId, bypassOwnershipCheck: isAdmin, cancellationToken);
        return Ok(ingredient);

    }



    [HttpPost("/api/ingredients")]
    public async Task<IActionResult> CreateIngredientAsync([FromBody] CreateIngredientRequest request, CancellationToken cancellationToken)
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

        Console.WriteLine($"Authenticated User ID: {appUserId}");

        var requestDto = new CreateIngredientRequestDto
        {
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            NutritionValues = new NutritionValuesRequestDto
            {
                Calories = request.NutritionValues.Calories,
                Protein = request.NutritionValues.Protein,
                Carbohydrates = request.NutritionValues.Carbohydrates,
                Fats = request.NutritionValues.Fats,
                Sugars = request.NutritionValues.Sugars,
                Fiber = request.NutritionValues.Fiber,
                Sodium = request.NutritionValues.Sodium
            },
            DietaryInfo = new DietaryInfoRequestDto
            {
                IsVegetarian = request.DietaryInfo.IsVegetarian,
                IsVegan = request.DietaryInfo.IsVegan,
                IsGlutenFree = request.DietaryInfo.IsGlutenFree,
                IsPescatarian = request.DietaryInfo.IsPescatarian
            },
            CoverImage = request.CoverImage,
            ExpirationDays = request.ExpirationDays,
            Details = request.Details.Select(detail => new DetailRequestDto
            {
                Order = detail.Order,
                Title = detail.Title,
                Description = detail.Description,
                MediaUrls = detail.MediaUrls
            }).ToList()
        };

        Guid id;

        if (request.AcceptImmediately)
        {
            id = await _ingredientService.AcceptIngredientAsync(appUserId, requestDto, cancellationToken);
        }
        else
        {
            id = await _ingredientService.CreateIngredientAsync(appUserId, requestDto, cancellationToken)
                ?? throw new Exception("Failed to create ingredient request.");
        }
        return Ok(id);
    }

    [Authorize]
    [HttpGet("/api/user/ingredients")]
    public async Task<IActionResult> GetUserIngredientsAsync(CancellationToken cancellationToken)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);

        var ingredients = await _ingredientService.GetUserIngredientsAsync(siteUserId, CancellationToken.None);
        return Ok(ingredients);
    }

    [AllowAnonymous]
    [HttpGet("/api/ingredients")]
    public async Task<IActionResult> GetIngredientsAsync(
    [FromQuery] string? category,
    [FromQuery] int? count,
    CancellationToken cancellationToken)
    {
        if (count.HasValue && count.Value <= 0)
        {
            return BadRequest("Count must be greater than zero.");
        }

        IEnumerable<IngredientDto> ingredients;

        if (!string.IsNullOrWhiteSpace(category))
        {
            ingredients = await _ingredientService.GetIngredientsByCategoryAsync(category, count ?? 10, cancellationToken);
        }
        else
        {
            ingredients = await _ingredientService.GetRandomIngredientsAsync(count ?? 10, cancellationToken);
        }

        return Ok(ingredients);
    }

    [AllowAnonymous]
    [HttpGet("/api/ingredients/summary")]
    public async Task<IActionResult> GetIngredientsOverviewAsync(CancellationToken cancellationToken)
    {
        var ingredientsOverview = await _ingredientService.GetIngredientsOverviewAsync(cancellationToken);
        return Ok(ingredientsOverview);
    }


}