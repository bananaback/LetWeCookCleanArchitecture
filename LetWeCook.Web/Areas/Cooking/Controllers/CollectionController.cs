using System.Security.Claims;
using LetWeCook.Application.Interfaces;
using LetWeCook.Areas.Cooking.Models.Requests;
using LetWeCook.Infrastructure.Persistence;
using LetWeCook.Web.Areas.Cooking.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Areas.Cooking.Controllers;

[Area("Cooking")]
public class CollectionController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICollectionService _collectionService;
    public CollectionController(UserManager<ApplicationUser> userManager, ICollectionService collectionService)
    {
        _userManager = userManager;
        _collectionService = collectionService;
    }

    [HttpGet("/api/collections")]
    [Authorize]
    public async Task<IActionResult> GetCollections(CancellationToken cancellationToken = default)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);
        var collections = await _collectionService.GetRecipeCollectionsAsync(siteUserId, cancellationToken);
        return Ok(collections);
    }

    [HttpPost("/api/collection/{collectionId}/items")]
    [Authorize]
    public async Task<IActionResult> AddRecipeToCollection([FromBody] AddRecipeToCollectionRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var siteUserId = await GetSiteUserId(cancellationToken);
            if (request.IsNewCollection)
            {
                await _collectionService.AddRecipeToNewCollectionAsync(siteUserId, request.RecipeId, request.CollectionName, cancellationToken);
            }
            else
            {
                await _collectionService.AddRecipeToCollectionAsync(siteUserId, request.RecipeId, request.CollectionId, cancellationToken);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            // Log the exception (not implemented here)
            return StatusCode(500, "An error occurred while adding recipe to collection." + ex.Message);
        }
    }

    [HttpPut("/api/collection/{collectionId}")]
    [Authorize]
    public async Task<IActionResult> UpdateCollectionName(Guid collectionId, [FromBody] string newName, CancellationToken cancellationToken = default)
    {

        var siteUserId = await GetSiteUserId(cancellationToken);
        await _collectionService.UpdateCollectionNameAsync(siteUserId, collectionId, newName, cancellationToken);
        return Ok();
    }

    [HttpDelete("/api/collection/{collectionId}")]
    [Authorize]
    public async Task<IActionResult> DeleteCollection(Guid collectionId, CancellationToken cancellationToken = default)
    {

        var siteUserId = await GetSiteUserId(cancellationToken);
        // Assuming there's a method in ICollectionService to delete a collection
        await _collectionService.DeleteCollectionAsync(siteUserId, collectionId, cancellationToken);
        return Ok();

    }

    [HttpDelete("/api/collection/{collectionId}/recipes/{recipeId}")]
    [Authorize]
    public async Task<IActionResult> RemoveRecipeFromCollection(Guid collectionId, Guid recipeId, CancellationToken cancellationToken = default)
    {
        var request = new RemoveRecipeFromCollectionRequest
        {
            CollectionId = collectionId,
            RecipeId = recipeId
        };

        var siteUserId = await GetSiteUserId(cancellationToken);
        await _collectionService.RemoveRecipeFromCollectionAsync(siteUserId, request.RecipeId, request.CollectionId, cancellationToken);
        return Ok();

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
}