namespace Domain.Drivers.ValueObjects;

/// <summary>
/// Running average of a driver's trip ratings. Immutable: every new
/// rating returns a new instance, which EF Core maps as an owned type
/// (no identity of its own).
/// </summary>
public sealed class DriverRating
{
    public double Value { get; private set; }
    public int Count { get; private set; }

    private DriverRating() { } // EF Core

    private DriverRating(double value, int count)
    {
        // Two decimals are enough for a 1..5 average and keep the stored
        // value stable across repeated additions (floating point drift).
        Value = Math.Round(value, 2, MidpointRounding.AwayFromZero);
        Count = count;
    }

    /// <summary>
    /// A newly hired driver starts at the top of the scale with zero
    /// ratings; the first real rating replaces the placeholder average.
    /// </summary>
    public static DriverRating CreateNew() => new(5.0, 0);

    public DriverRating AddRating(double newRating)
    {
        var newCount = Count + 1;
        var newValue = ((Value * Count) + newRating) / newCount;

        return new DriverRating(newValue, newCount);
    }
}
