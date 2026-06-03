public sealed record ClaimAmount
{
    public decimal Value { get; }
    public string Currency { get; }

    private ClaimAmount(decimal value, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty");

        if (value < 0)
            throw new ArgumentException("Amount cannot be negative");

        Value = decimal.Round(value, 2);
        Currency = currency.ToUpperInvariant();
    }

    public static ClaimAmount Of(decimal value, string currency)
        => new(value, currency);

    public bool IsPositive() => Value > 0;

    public bool IsGreaterThan(ClaimAmount other)
        => EnsureSameCurrency(other) && Value > other.Value;

    public ClaimAmount Add(ClaimAmount other)
    {
        EnsureSameCurrency(other);
        return new ClaimAmount(Value + other.Value, Currency);
    }

    public ClaimAmount Subtract(ClaimAmount other)
    {
        EnsureSameCurrency(other);

        if (other.Value > Value)
            throw new InvalidOperationException("Cannot subtract greater amount");

        return new ClaimAmount(Value - other.Value, Currency);
    }

    private bool EnsureSameCurrency(ClaimAmount other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Currency mismatch");

        return true;
    }
}