using Domain.SharedKernel;
using Domain.Shipments.Enums;

namespace Domain.Shipments.ValueObjects;

/// <summary>
/// Package dimensions. All three sides must be positive; validation lives in
/// the <see cref="Create"/> factory and returns a <see cref="Result{T}"/>.
/// </summary>
public sealed record Dimensions
{
    public decimal Length { get; init; }
    public decimal Width { get; init; }
    public decimal Height { get; init; }
    public DimensionUnit Unit { get; init; }

    private Dimensions() { } // EF Core

    private Dimensions(decimal length, decimal width, decimal height, DimensionUnit unit)
    {
        Length = length;
        Width = width;
        Height = height;
        Unit = unit;
    }

    public static Result<Dimensions> Create(decimal length, decimal width, decimal height, DimensionUnit unit)
    {
        if (length <= 0 || width <= 0 || height <= 0)
            return Result<Dimensions>.Failure(Invalid);

        return Result<Dimensions>.Success(new Dimensions(length, width, height, unit));
    }

    /// <summary>Convenience factory for code that already knows the values are valid.</summary>
    public static Dimensions InCentimeters(decimal length, decimal width, decimal height) =>
        FromResult(Create(length, width, height, DimensionUnit.Cm), nameof(length));

    /// <summary>Convenience factory for code that already knows the values are valid.</summary>
    public static Dimensions InInches(decimal length, decimal width, decimal height) =>
        FromResult(Create(length, width, height, DimensionUnit.Inch), nameof(length));

    /// <summary>Volumetric weight in KG using the industry standard 5000 divisor.</summary>
    public decimal VolumetricWeightKg => Unit switch
    {
        DimensionUnit.Cm => Length * Width * Height / 5000m,
        DimensionUnit.Inch => Length * Width * Height * 16.387m / 5000m,
        _ => throw new InvalidOperationException($"Unknown dimension unit '{Unit}'.")
    };

    public override string ToString() => $"{Length}x{Width}x{Height} {Unit}";

    public static readonly Error Invalid = new(
        "Dimensions.Invalid",
        "All dimensions must be greater than zero.");

    private static Dimensions FromResult(Result<Dimensions> result, string parameterName) =>
        result.IsSuccess
            ? result.Value
            : throw new ArgumentException(result.Error.Message, parameterName);
}
