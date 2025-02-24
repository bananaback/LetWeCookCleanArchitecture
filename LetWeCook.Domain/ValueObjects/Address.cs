using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.ValueObjects;

public class Address : ValueObject
{
    public string HouseNumber { get; private set; } = string.Empty;
    public string Street { get; private set; } = string.Empty;
    public string Ward { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;
    public string ProvinceOrCity { get; private set; } = string.Empty;

    public static Address Empty => new Address(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

    private Address() { } // For EF Core

    public Address(string houseNumber, string street, string ward, string district, string provinceOrCity)
    {
        HouseNumber = houseNumber ?? "";
        Street = street ?? "";
        Ward = ward ?? "";
        District = district ?? "";
        ProvinceOrCity = provinceOrCity ?? "";
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return HouseNumber;
        yield return Street;
        yield return Ward;
        yield return District;
        yield return ProvinceOrCity;
    }

    public bool IsEmpty => string.IsNullOrEmpty(HouseNumber) && string.IsNullOrEmpty(Street) && string.IsNullOrEmpty(Ward) && string.IsNullOrEmpty(District) && string.IsNullOrEmpty(ProvinceOrCity);

    public override string ToString()
    {
        var parts = new[] { HouseNumber, Street, Ward, District, ProvinceOrCity }
            .Where(p => !string.IsNullOrEmpty(p));
        return string.Join(", ", parts);
    }
}