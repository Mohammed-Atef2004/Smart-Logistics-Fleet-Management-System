using Domain.SharedKernel;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles.Events;

/// <summary>
/// Raised by <see cref="Vehicle.CompleteMaintenance"/> when a scheduled
/// maintenance is performed. Was internal with a payload (type, cost)
/// that no code could ever produce; it is now public and raised for real.
/// </summary>
public record MaintenancePerformedEvent : DomainEvent
{
    public VehicleId VehicleId { get; init; }
    public Guid ScheduleId { get; init; }

    public MaintenancePerformedEvent(VehicleId vehicleId, Guid scheduleId)
    {
        VehicleId = vehicleId;
        ScheduleId = scheduleId;
    }
}
