using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Controllers;

public class TestAuthController : Controller
{
    public IActionResult AdminOnly()
    {
        return View();
    }

    [Authorize(Roles = "User")] // Only Users can access
    public IActionResult UserOnly()
    {
        return View();
    }
}