using Domain.SharedKernel;
using Domain.Shipments.Errors;
using Domain.Shipments.Rules;
using Domain.Shipments.ValueObjects;

namespace Domain.Shipments;

/// <summary>
/// A package inside a shipment. It is an entity owned by the
/// <see cref="Shipment"/> aggregate: it is created and mutated only through
/// the aggregate root.
/// </summary>
public sealed class Package
{
    public PackageId Id { get; private set; }
    public string Description { get; private set; } = default!;
    public Weight Weight { get; private set; } = default!;
    public Dimensions Dimensions { get; private set; } = default!;
    public string? ContentCategory { get; private set; }
    public bool IsFragile { get; private set; }
    public bool RequiresRefrigeration { get; private set; }
    public decimal DeclaredValue { get; private set; }
    public string Currency { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    private Package() { } // EF Core

    private Package(
        PackageId id,
        string description,
        Weight weight,
        Dimensions dimensions,
        string? contentCategory,
        bool isFragile,
        bool requiresRefrigeration,
        decimal declaredValue,
        string currency)
    {
        Id = id;
        Description = description;
        Weight = weight;
        Dimensions = dimensions;
        ContentCategory = contentCategory;
        IsFragile = isFragile;
        RequiresRefrigeration = requiresRefrigeration;
        DeclaredValue = declaredValue;
        Currency = currency;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Package> Create(
        string description,
        Weight weight,
        Dimensions dimensions,
        string? contentCategory = null,
        bool isFragile = false,
        bool requiresRefrigeration = false,
        decimal declaredValue = 0,
        string currency = "USD")
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result<Package>.Failure(PackageErrors.EmptyDescription);

        if (weight is null || weight.Value <= 0)
            return Result<Package>.Failure(PackageErrors.InvalidWeight);

        if (dimensions is null
            || dimensions.Length <= 0
            || dimensions.Width <= 0
            || dimensions.Height <= 0)
            return Result<Package>.Failure(PackageErrors.InvalidDimensions);

        if (string.IsNullOrWhiteSpace(currency))
            return Result<Package>.Failure(PackageErrors.EmptyCurrency);

        var declaredValueRule = new DeclaredValueMustNotBeNegativeRule(declaredValue);
        if (declaredValueRule.IsBroken())
            return Result<Package>.Failure(declaredValueRule.Error);

        var package = new Package(
            PackageId.New(),
            description.Trim(),
            weight,
            dimensions,
            string.IsNullOrWhiteSpace(contentCategory) ? null : contentCategory.Trim(),
            isFragile,
            requiresRefrigeration,
            declaredValue,
            currency.Trim().ToUpperInvariant());

        return Result<Package>.Success(package);
    }

    /// <summary>
    /// Updates the mutable package details. Used by the aggregate; the return
    /// type is a plain <see cref="Result"/> because no value is produced.
    /// </summary>
    internal Result UpdateDetails(string description, Weight weight, Dimensions dimensions)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure(PackageErrors.EmptyDescription);

        if (weight is null || weight.Value <= 0)
            return Result.Failure(PackageErrors.InvalidWeight);

        if (dimensions is null
            || dimensions.Length <= 0
            || dimensions.Width <= 0
            || dimensions.Height <= 0)
            return Result.Failure(PackageErrors.InvalidDimensions);

        Description = description.Trim();
        Weight = weight;
        Dimensions = dimensions;

        return Result.Success();
    }

    internal void MarkAsFragile() => IsFragile = true;

    internal void MarkAsRequiresRefrigeration() => RequiresRefrigeration = true;

    public decimal BillableWeightKg =>
        Math.Max(Weight.ToKilograms(), Dimensions.VolumetricWeightKg);
}
