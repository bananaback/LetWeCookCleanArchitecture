using LetWeCook.Application.Interfaces;
using LetWeCook.Infrastructure.Persistence;
using LetWeCook.Web.Areas.Cooking.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LetWeCook.Web.Areas.Cooking.Controllers;

[Area("Cooking")]
public class DonationController : Controller
{
    private readonly IDonationService _donationService;
    private readonly ILogger<DonationController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public DonationController(IDonationService donationService,
    UserManager<ApplicationUser> userManager,
    ILogger<DonationController> logger)
    {
        _donationService = donationService;
        _userManager = userManager;
        _logger = logger;
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
            var (success, transactionId, customId, error) = await _donationService.CaptureDonationAsync(token);

            _logger.LogInformation($"Success: {success}, TransactionId: {transactionId}, CustomId: {customId}, Error: {error}");

            // Build absolute URL
            string redirectUrl = success
                ? $"https://localhost:7212/Cooking/Donation/ThankYou?id={customId}"
                : $"https://localhost:7212/Cooking/Donation/Sorry?id={customId}";

            return Redirect(redirectUrl);
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

    public IActionResult ThankYou(Guid id)
    {
        ViewData["DonationId"] = id;
        return View();
    }

    public IActionResult Sorry(Guid id)
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

    // get completed donations by recipe id
    [HttpGet("/api/donations/recipe/{recipeId}")]
    public async Task<IActionResult> GetCompletedDonationsByRecipeId(Guid recipeId, CancellationToken cancellationToken = default)
    {
        var donations = await _donationService.GetCompletedDonationsByRecipeIdAsync(recipeId, cancellationToken);

        return Ok(donations);
    }
}