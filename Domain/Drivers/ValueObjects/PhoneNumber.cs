using Domain.Common;
using Domain.Common.Domain.Common;
using Domain.SharedKernel;// Fixed the double namespace issue

namespace Domain.Drivers.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string CountryCode { get; private set; }
    public string Number { get; private set; }

    private PhoneNumber(string countryCode, string number)
    {
        CountryCode = countryCode;
        Number = number;
    }

    private PhoneNumber() { }

    public static Result<PhoneNumber> Create(string countryCode, string number)
    {
        if (string.IsNullOrWhiteSpace(countryCode) || !countryCode.StartsWith("+"))
            return Result<PhoneNumber>.Failure(new Error("PhoneNumber.InvalidCountryCode", "Invalid country code format."));

        if (string.IsNullOrWhiteSpace(number) || number.Length < 7)
            return Result<PhoneNumber>.Failure(new Error("PhoneNumber.InvalidNumber", "Phone number is too short."));

        return Result<PhoneNumber>.Success(new PhoneNumber(countryCode, number));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return Number;
    }

    public override string ToString() => $"{CountryCode}{Number}";
}