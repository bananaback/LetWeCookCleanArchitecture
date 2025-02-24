using LetWeCook.Domain.Common;
using LetWeCook.Domain.Enums;
using LetWeCook.Domain.ValueObjects;

namespace LetWeCook.Domain.Entities;

public class UserProfile : Entity
{
    public Guid UserId { get; private set; }
    public Name Name { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = string.Empty;
    public DateTime? BirthDate { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public Gender Gender { get; private set; }
    public Address Address { get; private set; } = null!;

    private UserProfile() : base() { } // For EF Core

    public UserProfile(Guid userId, Name name, string phoneNumber, string? profilePicture, DateTime? birthDate = null, Gender gender = Gender.Unspecified, Address? address = null) : base(Guid.NewGuid())
    {
        UserId = userId;
        Name = name;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate;
        ProfilePictureUrl = profilePicture;
        Gender = gender;
        Address = address ?? Address.Empty;
    }

    public void UpdateName(string firstName, string lastName)
    {
        Name = new Name(firstName, lastName);
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        PhoneNumber = phoneNumber;
    }

    public void UpdateBirthDate(DateTime? birthDate)
    {
        BirthDate = birthDate;
    }

    public void UpdateGender(Gender gender)
    {
        Gender = gender;
    }

    public void UpdateAddress(string houseNumber, string street, string ward, string district, string provinceOrCity)
    {
        Address = new Address(houseNumber, street, ward, district, provinceOrCity);
    }

}