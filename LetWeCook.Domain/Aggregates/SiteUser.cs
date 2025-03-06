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

    public void SetProfile(Name name, string phoneNumber, string? profilePicture = null, DateTime? birthDate = null, Gender gender = Gender.Unspecified, Address? address = null)
    {
        Profile = new UserProfile(Id, name, phoneNumber, profilePicture, birthDate, gender, address);
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