using System.ComponentModel.DataAnnotations;
using LetWeCook.Web.Areas.Identity.ViewModelValidators;

namespace LetWeCook.Web.Areas.Identity.Models.ViewModels;

public class RegisterViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [CustomPasswordValidation]
    public string Password { get; set; } = string.Empty;



    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}