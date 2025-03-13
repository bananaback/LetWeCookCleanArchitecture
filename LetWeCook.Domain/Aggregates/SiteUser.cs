using System.Reflection.Metadata;
using LetWeCook.Domain.Common;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;
using LetWeCook.Domain.Events;
using LetWeCook.Domain.ValueObjects;

namespace LetWeCook.Domain.Aggregates;

public class SiteUser : AggregateRoot
{
    public bool IsRemoved { get; private set; }
    public DateTime DateJoined { get; private set; } = DateTime.UtcNow;
    public UserProfile? Profile { get; private set; }

    private SiteUser() : base() { } // For EF Core

    public SiteUser(string email, bool verify)
    {
        Id = Guid.NewGuid();

        // Embed the UserRegisteredEvent in the constructor
        AddDomainEvent(new UserRegisteredEvent(Id, email, verify));
    }

    public void UpdateProfile(
        Name name,
        DateTime birthDate,
        string email,
        Gender gender,
        Address address,
        List<string> dietaryPreferences,
        List<DietaryPreference> allPreferences, // Pass all preferences for validation
        string? bio = null,
        string? facebook = null,
        string? instagram = null,
        string? phoneNumber = null,
        string? profilePicture = null
    )
    {
        if (IsRemoved)
        {
            throw new InvalidOperationException("Cannot update profile for a removed user.");
        }

        if (Profile == null)
        {
            // Create a new UserProfile if none exists
            Profile = new UserProfile(
                name,
                birthDate,
                gender,
                email,
                address,
                bio,
                facebook,
                instagram,
                phoneNumber,
                profilePicture
            );
        }
        else
        {
            // Update existing profile
            Profile.UpdateProfile(
                name,
                birthDate,
                gender,
                Profile.Email, // Keep existing email
                address,
                bio,
                facebook,
                instagram,
                phoneNumber,
                profilePicture
            );
        }
        Profile.UserId = Id;
        // Update dietary preferences
        Profile.UpdateDietaryPreferences(dietaryPreferences, allPreferences);
    }


    public void Remove()
    {
        if (IsRemoved)
        {
            throw new InvalidOperationException("User is already removed.");
        }
        IsRemoved = true;
    }

    public void RecordLogin(string email)
    {
        if (IsRemoved) throw new InvalidOperationException("Cannot record login for removed user.");
        AddDomainEvent(new UserLoggedInEvent(Id, email ?? "Unknown")); // Email might need adjustment
    }
}