using Domain.SharedKernel;

namespace Domain.Shipments.Errors;

/// <summary>
/// Single source of truth for package errors. Handlers and the aggregate use
/// these instead of building ad-hoc <see cref="Error"/> instances, otherwise
/// the same failure ends up with different codes per endpoint.
/// </summary>
public static class PackageErrors
{
    public static readonly Error EmptyDescription = new(
        "Package.EmptyDescription",
        "Package description cannot be empty.");

    public static readonly Error InvalidWeight = new(
        "Package.InvalidWeight",
        "Package weight must be greater than zero.");

    public static readonly Error InvalidDimensions = new(
        "Package.InvalidDimensions",
        "Package dimensions must be greater than zero.");

    public static readonly Error InvalidDeclaredValue = new(
        "Package.InvalidDeclaredValue",
        "Declared value cannot be negative.");

    public static readonly Error EmptyCurrency = new(
        "Package.EmptyCurrency",
        "Currency cannot be empty.");

    public static Error NotFound(Guid packageId) => new(
        "Package.NotFound",
        $"Package with ID '{packageId}' was not found in the shipment.");
}
