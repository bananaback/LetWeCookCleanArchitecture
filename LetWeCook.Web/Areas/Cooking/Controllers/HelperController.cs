using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Areas.Cooking.Controllers;

[Area("Cooking")]
public class HelperController : Controller
{
    public IActionResult WhatToCook()
    {
        return View();
    }
}