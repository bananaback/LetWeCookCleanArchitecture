using System.Threading.Tasks;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Areas.Cooking.Controllers;

[Area("Cooking")]
public class RecipeController : Controller
{
    private readonly IRecipeService _recipeService;
    public RecipeController(IRecipeService recipeService)
    {
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

}