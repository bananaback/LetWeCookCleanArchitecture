using LetWeCook.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Areas.AdminPanel.Controllers;

[Area("AdminPanel")]
[Authorize(Roles = "Admin")]
public class RequestEvaluationController : Controller
{
    private readonly IRequestService _requestService;

    public RequestEvaluationController(IRequestService requestService)
    {
        _requestService = requestService;
    }
}