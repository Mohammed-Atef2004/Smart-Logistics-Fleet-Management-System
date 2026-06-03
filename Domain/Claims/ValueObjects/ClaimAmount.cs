namespace Domain.Claims.ValueObjects;

public record ClaimAmount(decimal Value, string Currency)
{
    public static ClaimAmount Of(decimal value, string currency) => new(value, currency);

    public bool IsPositive() => Value > 0;

    public bool ExceedsLimit(ClaimAmount other) => Value > other.Value;

    public override string ToString() => $"{Value} {Currency}";
}
