namespace LetWeCook.Application.DTOs.Profile;

public class UserProfileDTO
{
    public Guid SiteUserId { get; set; }

    // Mandatory properties
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Address (flattened)
    public string HouseNumber { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string ProvinceOrCity { get; set; } = string.Empty;

    // Optional properties
    public string? Bio { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePic { get; set; }

    // Only store dietary preference names
    public List<string> DietaryPreferences { get; set; } = new();
}
