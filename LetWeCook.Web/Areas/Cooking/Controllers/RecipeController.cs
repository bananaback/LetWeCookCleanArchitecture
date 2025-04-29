using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Areas.Cooking.Controllers;

[Area("Cooking")]
public class RecipeController : Controller
{
    public IActionResult Editor()
    {
        return View();
    }
}