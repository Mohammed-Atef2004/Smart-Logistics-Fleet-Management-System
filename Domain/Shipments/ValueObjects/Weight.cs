using Domain.SharedKernel;
using Domain.Shipments.Enums;

namespace Domain.Shipments.ValueObjects;

/// <summary>
/// Weight value object. The factory returns a <see cref="Result{T}"/> so the
/// caller decides how to surface an invalid weight; the private constructor
/// never throws.
/// </summary>
public sealed record Weight
{
    public decimal Value { get; init; }
    public WeightUnit Unit { get; init; }

    private Weight() { } // EF Core

    private Weight(decimal value, WeightUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public static Result<Weight> Create(decimal value, WeightUnit unit)
    {
        if (value <= 0)
            return Result<Weight>.Failure(InvalidValue);

        return Result<Weight>.Success(new Weight(value, unit));
    }

    /// <summary>Convenience factory for code that already knows the value is valid.</summary>
    public static Weight InKilograms(decimal value) => FromResult(Create(value, WeightUnit.Kg), nameof(value));

    /// <summary>Convenience factory for code that already knows the value is valid.</summary>
    public static Weight InGrams(decimal value) => FromResult(Create(value, WeightUnit.Gram), nameof(value));

    /// <summary>Convenience factory for code that already knows the value is valid.</summary>
    public static Weight InPounds(decimal value) => FromResult(Create(value, WeightUnit.Pound), nameof(value));

    public decimal ToKilograms() => Unit switch
    {
        WeightUnit.Kg => Value,
        WeightUnit.Gram => Value / 1000m,
        WeightUnit.Pound => Value * 0.453592m,
        _ => throw new InvalidOperationException($"Unknown weight unit '{Unit}'.")
    };

    public override string ToString() => $"{Value} {Unit}";

    public static readonly Error InvalidValue = new(
        "Weight.InvalidValue",
        "Weight must be greater than zero.");

    private static Weight FromResult(Result<Weight> result, string parameterName) =>
        result.IsSuccess
            ? result.Value
            : throw new ArgumentException(result.Error.Message, parameterName);
}
