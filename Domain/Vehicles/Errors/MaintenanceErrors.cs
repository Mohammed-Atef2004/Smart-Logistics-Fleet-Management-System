using Domain.SharedKernel;
using Domain.Vehicles.Enums;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles.Errors;

public static class MaintenanceErrors
{
    public static Error ScheduleNotFound(Guid scheduleId) =>
        new("Maintenance.ScheduleNotFound", $"Maintenance schedule with ID '{scheduleId}' was not found.");

    public static Error DescriptionRequired =>
        new("Maintenance.DescriptionRequired", "Description is required");

    public static Error DescriptionTooLong =>
        new("Maintenance.DescriptionTooLong", $"Description cannot exceed {MaintenanceDescription.MaxLength} characters.");

    public static Error RemarksTooLong =>
        new("Maintenance.RemarksTooLong", $"Remarks cannot exceed {MaintenanceRemarks.MaxLength} characters.");

    public static Error DateInPast =>
        new("Maintenance.DateInPast", "Maintenance date cannot be in the past");

    public static Error AlreadyCompleted =>
        new("Maintenance.AlreadyCompleted", "Maintenance is already completed");

    public static Error CannotRescheduleCompleted =>
        new("Maintenance.CannotRescheduleCompleted", "Cannot reschedule completed maintenance");
}
