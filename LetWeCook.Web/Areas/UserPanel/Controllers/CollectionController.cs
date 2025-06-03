using System.Security.Claims;
using LetWeCook.Application.DTOs.RecipeCollections;
using LetWeCook.Application.Interfaces;
using LetWeCook.Infrastructure.Persistence;
using LetWeCook.Web.Areas.UserPanel.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Areas.UserPanel.Controllers;

[Area("UserPanel")]
public class CollectionController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICollectionService _collectionService;

    public CollectionController(UserManager<ApplicationUser> userManager, ICollectionService collectionService)
    {
        _userManager = userManager;
        _collectionService = collectionService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("/api/collections-browse")]
    [Authorize]
    public async Task<IActionResult> BrowseCollections([FromBody] CollectionQueryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var siteUserId = await GetSiteUserId(cancellationToken);

            var result = await _collectionService.BrowseCollection(
                siteUserId,
                new RecipeCollectionQueryRequestDto
                {
                    SearchTerm = request.SearchTerm,
                    SortBy = request.SortBy,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    IsAscending = request.IsAscending
                },
                cancellationToken
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error" + ex.Message);
        }
    }

    [HttpPost("/api/collection-items-browse")]
    [Authorize]
    public async Task<IActionResult> BrowseCollectionItems([FromBody] CollectionItemQueryRequest request, CancellationToken cancellationToken = default)
    {
        //try
        //{
        var siteUserId = await GetSiteUserId(cancellationToken);

        var result = await _collectionService.BrowseCollectionItems(
            siteUserId,
            new RecipeCollectionItemQueryRequestDto
            {
                CollectionId = request.CollectionId,
                SortBy = request.SortBy,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                IsAscending = request.IsAscending,
                SearchTerm = request.SearchTerm
            },
            cancellationToken
        );

        return Ok(result);
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine("Error in BrowseCollectionItems: " + ex.Message);
        //    return StatusCode(500, "Internal server error" + ex.Message);
        //}
    }

    private async Task<Guid> GetSiteUserId(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        Console.WriteLine("User ID Claim: " + userIdClaim?.Value);
        if (userIdClaim == null) throw new UnauthorizedAccessException("User ID not found in claims.");
        var appUser = await _userManager.FindByIdAsync(userIdClaim.Value);
        if (appUser == null) throw new UnauthorizedAccessException("User not found in database.");
        return appUser.SiteUserId;
    }

    public IActionResult CollectionDetails(Guid collectionId)
    {
        if (collectionId == Guid.Empty)
        {
            return BadRequest("Invalid collection ID.");
        }

        ViewData["CollectionId"] = collectionId;

        return View();
    }
}