using System.Threading.Tasks;
using LetWeCook.Application.DTOs.Authentication;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Exceptions;
using LetWeCook.Web.Areas.Identity.Models.DTOs;
using LetWeCook.Web.Areas.Identity.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthenticationService = LetWeCook.Application.Interfaces.IAuthenticationService;

namespace LetWeCook.Web.Areas.Identity.Controllers;

[Area("Identity")]
public class AccountController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAuthenticationService authenticationService,
        IIdentityService identityService,
        ILogger<AccountController> logger)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _identityService = identityService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous] // Ensure anyone can access this
    public IActionResult AccessDenied(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View(new LoginViewModel()); // Ensure ViewModel is initialized
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            var requestDto = new LoginRequestDTO
            {
                Email = viewModel.Email,
                Password = viewModel.Password
            };

            var response = await _authenticationService.LoginAsync(requestDto, HttpContext.RequestAborted);
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        catch (AuthenticationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(viewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _authenticationService.LogoutAsync(HttpContext.RequestAborted);
            _logger.LogInformation("User logged out successfully.");
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout.");
            return RedirectToAction("Index", "Home", new { area = "" }); // Redirect anyway, but log the issue
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel()); // Ensure ViewModel is initialized
    }

    [HttpGet]
    public IActionResult RegistrationSuccess(string email)
    {
        ViewData["Email"] = email;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            var requestDto = new RegisterRequestDTO
            {
                Username = viewModel.Username,
                Email = viewModel.Email,
                Password = viewModel.Password
            };

            var response = await _authenticationService.RegisterAsync(requestDto, HttpContext.RequestAborted);
            return RedirectToAction("RegistrationSuccess", new { email = response.Email });
        }
        catch (UserRegistrationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(viewModel);
        }
        catch (UserAlreadyExistsException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(viewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendEmailRequestDTO request)
    {
        try
        {
            // Call the service to resend the verification email
            await _authenticationService.ResendVerificationEmailAsync(request.Email);
            return Ok(new { message = "Verification email resent successfully!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to resend email: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> VerifyEmail(string email, string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("VerifyEmail called with missing email or token.");
            return RedirectToAction("VerificationFailed");
        }

        try
        {
            var isConfirmed = await _identityService.ConfirmEmailAsync(email, token, HttpContext.RequestAborted);
            if (isConfirmed)
            {
                _logger.LogInformation("Email verified successfully for {Email}", email);
                return RedirectToAction("VerificationSuccess");
            }
            else
            {
                _logger.LogWarning("Email verification failed for {Email}", email);
                return RedirectToAction("VerificationFailed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying email for {Email}", email);
            return RedirectToAction("VerificationFailed");
        }
    }

    [HttpGet]
    public IActionResult VerificationSuccess()
    {
        return View();
    }

    [HttpGet]
    public IActionResult VerificationFailed()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            await _authenticationService.SendPasswordResetLinkAsync(viewModel.Email);
            return RedirectToAction("ForgotPasswordConfirmation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending forgot password email.");
            return RedirectToAction("ForgotPasswordConfirmation");
        }
    }

    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        var model = new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _authenticationService.ResetPasswordAsync(model.Email, model.Token, model.Password);
            return RedirectToAction("ResetPasswordConfirmation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "Invalid or expired token.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLogin(string provider, string? returnUrl)
    {
        // No need to deal with AuthenticationProperties here
        await _authenticationService.SignInWithExternalProviderAsync(provider, Url.Action("ExternalLoginCallback", "Account", new { returnUrl }));
        return new EmptyResult(); // Challenge redirects
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
    {
        var loginData = await _authenticationService.GetExternalLoginInfoAsync();
        if (loginData == null)
        {
            return RedirectToAction("Login"); // Authentication failed or was canceled
        }

        var success = await _authenticationService.RegisterExternalUserAsync(loginData, loginData.Email);
        if (!success)
        {
            return RedirectToAction("Login"); // Registration or sign-in failed
        }

        return Redirect(returnUrl); // User is signed in, redirect to original page
    }
}