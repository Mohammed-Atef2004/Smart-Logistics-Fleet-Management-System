using Domain.SharedKernel;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles.Events;

/// <summary>
/// Raised when a maintenance schedule is created or moved to a new date
/// (rescheduling re-raises it). The scheduled date travels in
/// <see cref="ScheduledDate"/> — handlers must log that, not the base
/// event's <see cref="DomainEvent.OccurredOnUtc"/>.
/// </summary>
public record MaintenanceScheduledEvent : DomainEvent
{
    public VehicleId VehicleId { get; init; }
    public Guid ScheduleId { get; init; }
    public DateTime ScheduledDate { get; init; }

    public MaintenanceScheduledEvent(VehicleId vehicleId, Guid scheduleId, DateTime scheduledDate)
    {
        VehicleId = vehicleId;
        ScheduleId = scheduleId;
        ScheduledDate = scheduledDate;
    }
}
