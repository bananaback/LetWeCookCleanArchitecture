using LetWeCook.Infrastructure.Persistence;
using LetWeCook.Web.Areas.UserPanel.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LetWeCook.Areas.UserPanel.Controllers;

[Area("UserPanel")]
[Authorize] // Ensure only authenticated users can access
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    // GET: /UserPanel/Profile
    public IActionResult Index()
    {
        return View();
    }


    [HttpPost("/api/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);

        return Ok();
    }

    private async Task<Guid> GetSiteUserId(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) throw new UnauthorizedAccessException("User ID not found in claims.");
        var appUser = await _userManager.FindByIdAsync(userIdClaim.Value);
        if (appUser == null) throw new UnauthorizedAccessException("User not found in database.");
        return appUser.SiteUserId;
    }
}
