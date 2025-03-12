namespace LetWeCook.Application.DTOs.Profile;

public class UpdateUserProfileRequestDTO
{
    public string? ProfilePicture { get; set; } // Optional

    public string FirstName { get; set; } = string.Empty; // Mandatory
    public string LastName { get; set; } = string.Empty; // Mandatory
    public string? Bio { get; set; } // Optional

    public DateTime BirthDate { get; set; } // Mandatory
    public string Gender { get; set; } = string.Empty; // Mandatory
    public string Email { get; set; } = string.Empty; // Mandatory
    public string? PhoneNumber { get; set; } // Optional
    public string? Instagram { get; set; } // Optional
    public string? Facebook { get; set; } // Optional

    public AddressDTO Address { get; set; } = new(); // Mandatory

    public List<string> DietaryPreferences { get; set; } = new(); // Optional
}

public class AddressDTO
{
    public string HouseNumber { get; set; } = string.Empty; // Mandatory
    public string Street { get; set; } = string.Empty; // Mandatory
    public string Ward { get; set; } = string.Empty; // Mandatory
    public string District { get; set; } = string.Empty; // Mandatory
    public string City { get; set; } = string.Empty; // Mandatory
    public string Province { get; set; } = string.Empty; // Mandatory
}
