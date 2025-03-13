using LetWeCook.Domain.Common;
using LetWeCook.Domain.Enums;
using LetWeCook.Domain.ValueObjects;

namespace LetWeCook.Domain.Entities;

public class UserProfile : Entity
{
    public Guid UserId { get; set; }

    // Mandatory properties (sorted alphabetically)
    public Address Address { get; private set; } = null!;
    public DateTime BirthDate { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public Gender Gender { get; private set; }
    public Name Name { get; private set; } = null!;

    // Optional properties (sorted alphabetically)
    public string? Bio { get; private set; }
    public string? Facebook { get; private set; }
    public string? Instagram { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? ProfilePic { get; private set; }

    private readonly List<DietaryPreference> _dietaryPreferences = new();
    public IReadOnlyList<DietaryPreference> DietaryPreferences => _dietaryPreferences.AsReadOnly();

    private UserProfile() : base() { } // For EF Core

    public UserProfile(
        Name name,
        DateTime birthDate,
        Gender gender,
        string email,
        Address address,
        string? bio = null,
        string? facebook = null,
        string? instagram = null,
        string? phoneNumber = null,
        string? profilePicture = null
    ) : base(Guid.NewGuid())
    {
        Name = name;
        BirthDate = birthDate;
        Gender = gender;
        Email = email;
        Address = address;
        Bio = bio;
        Facebook = facebook;
        Instagram = instagram;
        PhoneNumber = phoneNumber;
        ProfilePic = profilePicture;
    }

    public void UpdateProfile(
        Name name,
        DateTime birthDate,
        Gender gender,
        string email,
        Address address,
        string? bio = null,
        string? facebook = null,
        string? instagram = null,
        string? phoneNumber = null,
        string? profilePicture = null
    )
    {
        Name = name;
        BirthDate = birthDate;
        Gender = gender;
        Email = email;
        Address = address;
        Bio = bio;
        Facebook = facebook;
        Instagram = instagram;
        PhoneNumber = phoneNumber;
        ProfilePic = profilePicture;
    }

    public void UpdateDietaryPreferences(List<string> preferenceNames, List<DietaryPreference> allPreferences)
    {
        if (preferenceNames == null) throw new ArgumentNullException(nameof(preferenceNames));
        if (allPreferences == null) throw new ArgumentNullException(nameof(allPreferences));

        // Find preferences to keep
        var validPreferences = allPreferences.Where(dp => preferenceNames.Contains(dp.Name)).ToList();

        // Remove those not in the new list
        _dietaryPreferences.RemoveAll(dp => !validPreferences.Contains(dp));

        // Add missing preferences
        foreach (var preference in validPreferences)
        {
            if (!_dietaryPreferences.Contains(preference))
            {
                _dietaryPreferences.Add(preference);
            }
        }
    }
}
