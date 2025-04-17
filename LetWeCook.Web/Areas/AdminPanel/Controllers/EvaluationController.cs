using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Areas.AdminPanel.Controllers;

[Area("AdminPanel")]
[Authorize(Roles = "Admin")]
public class EvaluationController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}