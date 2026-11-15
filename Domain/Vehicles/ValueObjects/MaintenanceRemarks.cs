using Domain.SharedKernel;
using Domain.Vehicles.Errors;

namespace Domain.Vehicles.ValueObjects;

/// <summary>
/// Result-pattern value object for optional maintenance notes.
/// The old public constructor threw on long values; all validation now
/// lives in <see cref="Create"/>.
/// </summary>
public record MaintenanceRemarks
{
    public const int MaxLength = 500;

    public string? Value { get; init; }

    private MaintenanceRemarks(string value)
    {
        Value = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public static Result<MaintenanceRemarks> Create(string? value)
    {
        var trimmed = value?.Trim();

        if (string.IsNullOrEmpty(trimmed))
            return Result<MaintenanceRemarks>.Success(new MaintenanceRemarks(string.Empty));

        if (trimmed.Length > MaxLength)
            return Result<MaintenanceRemarks>.Failure(MaintenanceErrors.RemarksTooLong);

        return Result<MaintenanceRemarks>.Success(new MaintenanceRemarks(trimmed));
    }
}
