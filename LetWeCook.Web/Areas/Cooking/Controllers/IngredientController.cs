using LetWeCook.Application.DTOs.Ingredient;
using LetWeCook.Application.Interfaces;
using LetWeCook.Areas.Cooking.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Areas.Cooking.Controllers;

[Area("Cooking")]
public class IngredientController : Controller
{
    private readonly IIngredientService _ingredientService;
    public IngredientController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    public IActionResult Browser()
    {
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

    [AllowAnonymous]
    [HttpGet("api/ingredient-categories")]
    public async Task<IActionResult> GetAllIngredientCategoriesAsync()
    {
        var ingredientCategories = await _ingredientService.GetAllIngredientCategoriesAsync(CancellationToken.None);
        return Ok(ingredientCategories);
    }

    // Create get ingredient by guid endpoint
    [AllowAnonymous]
    [HttpGet("api/ingredients/{id}")]
    public async Task<IActionResult> GetIngredientAsync(Guid id, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientService.GetIngredientAsync(id, cancellationToken);
        return Ok(ingredient);
    }

    [HttpPost("api/ingredients")]
    public async Task<IActionResult> CreateIngredientAsync([FromBody] CreateIngredientRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest("Request body is required.");
        }

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

        var ingredientDto = await _ingredientService.CreateIngredientAsync(requestDto, cancellationToken);

        return Ok(ingredientDto);
    }

    [AllowAnonymous]
    [HttpGet("api/ingredients")]
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
    [HttpGet("api/ingredients/overview")]
    public async Task<IActionResult> GetIngredientsOverviewAsync(CancellationToken cancellationToken)
    {
        var ingredientsOverview = await _ingredientService.GetIngredientsOverviewAsync(cancellationToken);
        return Ok(ingredientsOverview);
    }


}