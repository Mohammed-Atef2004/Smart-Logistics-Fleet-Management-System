using Domain.SharedKernel;
using Domain.Vehicles.Errors;

namespace Domain.Vehicles.ValueObjects;

/// <summary>
/// Result-pattern value object: the only way in is <see cref="Create"/>,
/// which returns a Result instead of throwing like the old public
/// constructor did (throwing constructors broke the Result convention
/// used everywhere else in this codebase).
/// </summary>
public record MaintenanceDescription
{
    public const int MaxLength = 200;

    public string Value { get; init; } = default!;

    private MaintenanceDescription(string value)
    {
        Value = value;
    }

    public static Result<MaintenanceDescription> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<MaintenanceDescription>.Failure(MaintenanceErrors.DescriptionRequired);

        if (value.Length > MaxLength)
            return Result<MaintenanceDescription>.Failure(MaintenanceErrors.DescriptionTooLong);

        return Result<MaintenanceDescription>.Success(new MaintenanceDescription(value));
    }
}
