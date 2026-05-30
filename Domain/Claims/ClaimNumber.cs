namespace Domain.Claims.ValueObjects;

public record ClaimNumber(string Value)
{
    public static ClaimNumber Generate()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Guid.NewGuid().ToString("N")[..8].ToUpper();
        return new ClaimNumber($"CLM-{datePart}-{randomPart}");
    }

    public static ClaimNumber From(string value) => new(value);

    public override string ToString() => Value;
}
