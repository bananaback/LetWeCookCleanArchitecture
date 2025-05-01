using System.Security.Claims;
using LetWeCook.Application.Interfaces;
using LetWeCook.Infrastructure.Persistence;
using LetWeCook.Web.Areas.Cooking.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LetWeCook.Web.Areas.Cooking.Controllers;

[Area("Cooking")]
public class DonationController : Controller
{
    private readonly IDonationService _donationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DonationController(IDonationService donationService, UserManager<ApplicationUser> userManager)
    {
        _donationService = donationService;
        _userManager = userManager;
    }

    [HttpPost("/api/donation/{id:guid}")]
    public async Task<IActionResult> CreateDonation([FromBody] DonationRequest request, Guid id)
    {
        if (!ModelState.IsValid || request.Amount < 1.00m)
        {
            return BadRequest(new { message = "Invalid donation amount. Minimum is $1.00." });
        }


        // get app user id from authenticated context
        var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(appUserId))
        {
            return Unauthorized(new { message = "User not authenticated." });
        }

        try
        {
            // Call IPaymentService to create PayPal order
            var donationId = await _donationService.CreateDonationAsync(
                GetSiteUserId(appUserId),
                id,
                request.Amount,
                request.Currency,
                request.DonationMessage);


            return Ok(donationId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error creating donation: {ex.Message}" });
        }
    }

    [HttpGet("/api/donation/success")]
    public async Task<IActionResult> Success(string token, string PayerID)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(PayerID))
        {
            return BadRequest(new { message = "Missing token or PayerID." });
        }

        try
        {
            // Capture payment
            var (success, transactionId, customId, error) = await _donationService.CaptureDonationAsync(token);
            if (success)
            {
                return Ok(new { message = "Donation processed successfully!" });
            }
            else
            {
                return BadRequest(new { message = error ?? "Failed to capture donation." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error capturing donation: {ex.Message}" });
        }
    }

    public IActionResult DonationConfirmation(Guid id)
    {
        ViewData["DonationId"] = id;
        return View();
    }

    private Guid GetSiteUserId(string appUserId)
    {
        // parse to Guid
        if (!Guid.TryParse(appUserId, out var parsedAppUserId))
        {
            throw new InvalidOperationException("Invalid user ID format.");
        }
        var siteUser = _userManager.Users.FirstOrDefault(u => u.Id == parsedAppUserId);
        if (siteUser == null)
        {
            throw new InvalidOperationException("Site user not found.");
        }

        return siteUser.SiteUserId;
    }

    // get donation details
    [HttpGet("/api/donations/{id}")]
    public async Task<IActionResult> GetDonationDetails(Guid id, CancellationToken cancellationToken = default)
    {
        var donation = await _donationService.GetDonationDetailsAsync(id, cancellationToken);

        return Ok(donation);
    }
}