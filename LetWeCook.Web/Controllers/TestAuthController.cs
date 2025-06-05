using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Common;
using LetWeCook.Domain.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Controllers;

public class TestAuthController : Controller
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    public TestAuthController(IDomainEventDispatcher domainEventDispatcher)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }
    public IActionResult AdminOnly()
    {
        return View();
    }

    [Authorize(Roles = "User")] // Only Users can access
    public IActionResult UserOnly()
    {
        return View();
    }

    [HttpGet("/api/testhit")]
    public IActionResult TestHit(CancellationToken cancellationToken = default)
    {

        var newEvent = new RecipeSnapshotRequestedEvent();
        _domainEventDispatcher.DispatchEventsAsync(new List<DomainEvent> { newEvent }, cancellationToken);
        return Ok("Test hit successful");
    }
}