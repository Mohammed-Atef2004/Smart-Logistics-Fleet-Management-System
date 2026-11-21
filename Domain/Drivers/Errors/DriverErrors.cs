using Domain.SharedKernel;

namespace Domain.Drivers.Errors;

/// <summary>
/// Single source of truth for driver errors. Handlers must use these
/// instead of building ad-hoc <see cref="Error"/> instances, otherwise
/// the same failure ends up with different codes/messages per endpoint.
/// </summary>
public static class DriverErrors
{
    public static Error NotFound(Guid driverId) =>
        new("Driver.NotFound", $"Driver with ID '{driverId}' was not found.");

    public static readonly Error EmptyName =
        new("Driver.EmptyName", "Driver name is required.");

    public static readonly Error NameTooLong =
        new("Driver.NameTooLong", "Driver name must not exceed 100 characters.");

    public static readonly Error InvalidLicenseNumber =
        new("Driver.InvalidLicenseNumber", "Driver license number is invalid.");

    public static readonly Error LicenseNumberTooLong =
        new("Driver.LicenseNumberTooLong", "Driver license number must not exceed 20 characters.");

    public static readonly Error InvalidLicenseCategory =
        new("Driver.InvalidLicenseCategory", "Driver license category is invalid.");

    public static readonly Error LicenseExpired =
        new("Driver.LicenseExpired", "Driver license is expired.");

    public static readonly Error LicenseNumberAlreadyExists =
        new("Driver.LicenseNumberAlreadyExists", "A driver with the same license number already exists.");

    public static readonly Error NotActive =
        new("Driver.NotActive", "Driver must be active to perform this action.");

    public static readonly Error AlreadySuspended =
        new("Driver.AlreadySuspended", "Driver is already suspended.");

    public static readonly Error NotSuspended =
        new("Driver.NotSuspended", "Driver must be suspended before being reactivated.");

    public static readonly Error AlreadyTerminated =
        new("Driver.AlreadyTerminated", "Driver is already terminated.");

    public static readonly Error Terminated =
        new("Driver.Terminated", "A terminated driver can no longer change status.");

    public static readonly Error AlreadyInShift =
        new("Driver.AlreadyInShift", "Driver is already assigned to a shift.");

    public static readonly Error NotInShift =
        new("Driver.NotInShift", "Driver is not assigned to a shift.");

    public static readonly Error InvalidShift =
        new("Driver.InvalidShift", "A valid shift id is required.");

    public static readonly Error InvalidRating =
        new("Driver.InvalidRating", "Driver rating must be between 1 and 5.");
}
