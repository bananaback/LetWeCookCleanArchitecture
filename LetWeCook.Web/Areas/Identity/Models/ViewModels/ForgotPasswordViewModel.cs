using System.ComponentModel.DataAnnotations;

namespace LetWeCook.Web.Areas.Identity.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}