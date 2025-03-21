using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Areas.Cooking.Controllers;

[Area("Cooking")]
public class IngredientController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Editor()
    {
        return View();
    }
}