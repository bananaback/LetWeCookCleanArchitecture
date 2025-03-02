using System.ComponentModel.DataAnnotations;
using LetWeCook.Web.Areas.Identity.ViewModelValidators;

namespace LetWeCook.Web.Areas.Identity.Models.ViewModels;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [CustomPasswordValidation]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}