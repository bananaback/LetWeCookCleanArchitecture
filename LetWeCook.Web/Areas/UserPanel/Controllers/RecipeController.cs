using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Areas.UserPanel.Controllers;

[Area("UserPanel")]
public class RecipeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}