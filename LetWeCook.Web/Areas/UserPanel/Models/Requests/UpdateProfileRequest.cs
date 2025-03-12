using System.ComponentModel.DataAnnotations;

namespace LetWeCook.Web.Areas.UserPanel.Models.Requests
{
    public class UpdateProfileRequest
    {
        [Url]
        public string ProfilePicture { get; set; } = string.Empty; // Optional (users may not upload an image immediately)

        [Required, StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty; // Mandatory

        [Required, StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty; // Mandatory

        [StringLength(500)]
        public string Bio { get; set; } = string.Empty; // Optional (personal preference)

        [Required, DataType(DataType.Date)]
        public DateTime BirthDate { get; set; } // Mandatory (important for age-restricted features)

        [Required]
        public string Gender { get; set; } = string.Empty; // Mandatory (ensures personalized UX)

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty; // Mandatory (unique identifier)

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty; // Optional (not everyone wants to provide a phone)

        [Url]
        public string Instagram { get; set; } = string.Empty; // Optional (not everyone has social media)

        [Url]
        public string Facebook { get; set; } = string.Empty; // Optional (same as Instagram)

        [Required]
        public AddressRequest Address { get; set; } = new(); // Mandatory (important for localized features)

        public List<string> DietaryPreferences { get; set; } = new(); // Optional (some users have no preferences)
    }

    public class AddressRequest
    {
        [Required, StringLength(10)]
        public string HouseNumber { get; set; } = string.Empty; // Mandatory (helps with address validation)

        [Required, StringLength(100, MinimumLength = 5)]
        public string Street { get; set; } = string.Empty; // Mandatory

        [Required, StringLength(50, MinimumLength = 3)]
        public string Ward { get; set; } = string.Empty; // Mandatory

        [Required, StringLength(50, MinimumLength = 3)]
        public string District { get; set; } = string.Empty; // Mandatory

        [Required, StringLength(50, MinimumLength = 3)]
        public string City { get; set; } = string.Empty; // Mandatory

        [Required, StringLength(50, MinimumLength = 3)]
        public string Province { get; set; } = string.Empty; // Mandatory
    }
}
