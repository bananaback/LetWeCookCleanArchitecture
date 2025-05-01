using LetWeCook.Application.DTOs.Profile;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Enums;
using LetWeCook.Infrastructure.Persistence;
using LetWeCook.Web.Areas.UserPanel.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LetWeCook.Areas.UserPanel.Controllers;

[Area("UserPanel")]
[Authorize] // Ensure only authenticated users can access
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserProfileService _userProfileService;
    private readonly IRequestService _requestService;
    public ProfileController(UserManager<ApplicationUser> userManager, IUserProfileService userProfileService, IRequestService requestService)
    {
        _userManager = userManager;
        _userProfileService = userProfileService;
        _requestService = requestService;

    }

    // GET: /UserPanel/Profile
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Requests()
    {
        return View();
    }

    [HttpGet("/api/requests")]
    public async Task<IActionResult> GetRequests(CancellationToken cancellationToken = default)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);
        var userRoles = await GetUserRolesAsync(cancellationToken);

        // log user roles for debugging
        foreach (var role in userRoles)
        {
            Console.WriteLine($"User Role: {role}");
        }
        // check if user or admin
        if (userRoles.Contains("Admin"))
        {
            var requests = await _requestService.GetAllRequestsAsync(cancellationToken);
            return Ok(requests);
        }
        else if (userRoles.Contains("User"))
        {
            var requests = await _requestService.GetUserRequestsAsync(siteUserId, cancellationToken);
            return Ok(requests);
        }
        else
        {
            return Forbid(); // User does not have permission to access this resource
        }
    }

    [HttpGet("/api/dietary-preferences")]
    public async Task<IActionResult> GetDietaryPreferences(CancellationToken cancellationToken = default)
    {
        var dietaryPreferences = await _userProfileService.GetAllSystemDietaryPreferencesAsync(cancellationToken);
        return Ok(dietaryPreferences);
    }

    [HttpGet("/api/profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken = default)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);
        var userProfile = await _userProfileService.GetProfileAsync(siteUserId, cancellationToken);

        if (userProfile != null)
        {
            return Ok(userProfile);
        }

        // If userProfile is null, create a default DTO and attempt to get email from claims
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;

        var emptyProfile = new UserProfileDTO
        {
            SiteUserId = siteUserId,
            FirstName = string.Empty,
            LastName = string.Empty,
            BirthDate = DateTime.MinValue,
            Gender = Gender.Unspecified.ToString(),
            Email = email,
            HouseNumber = string.Empty,
            Street = string.Empty,
            Ward = string.Empty,
            District = string.Empty,
            ProvinceOrCity = string.Empty,
            Bio = string.Empty,
            Facebook = string.Empty,
            Instagram = string.Empty,
            PhoneNumber = string.Empty,
            PayPalEmail = string.Empty,
            ProfilePic = string.Empty,
            DietaryPreferences = new List<string>()
        };

        return Ok(emptyProfile);
    }



    [HttpPost("/api/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var siteUserId = await GetSiteUserId(cancellationToken);

        var updateUserProfileRequestDTO = new UpdateUserProfileRequestDTO
        {
            ProfilePicture = request.ProfilePicture,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Bio = request.Bio,
            BirthDate = request.BirthDate,
            Gender = request.Gender,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PayPalEmail = request.PayPalEmail,
            Instagram = request.Instagram,
            Facebook = request.Facebook,
            Address = new AddressDTO
            {
                HouseNumber = request.Address.HouseNumber,
                Street = request.Address.Street,
                Ward = request.Address.Ward,
                District = request.Address.District,
                ProvinceOrCity = request.Address.Province
            },
            DietaryPreferences = request.DietaryPreferences
        };

        var updatedProfile = await _userProfileService.UpdateProfileAsync(siteUserId, updateUserProfileRequestDTO, cancellationToken);

        return Ok(updatedProfile);
    }

    private async Task<Guid> GetSiteUserId(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) throw new UnauthorizedAccessException("User ID not found in claims.");
        var appUser = await _userManager.FindByIdAsync(userIdClaim.Value);
        if (appUser == null) throw new UnauthorizedAccessException("User not found in database.");
        return appUser.SiteUserId;
    }

    private async Task<List<string>> GetUserRolesAsync(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) throw new UnauthorizedAccessException("User ID not found in claims.");
        var user = await _userManager.FindByIdAsync(userIdClaim.Value);
        if (user == null) return new List<string>();
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }
}
