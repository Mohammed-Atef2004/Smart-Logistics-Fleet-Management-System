using Domain.SharedKernel;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles.Events;

public record VehicleRetiredEvent : DomainEvent
{
    public VehicleId VehicleId { get; init; }

    public VehicleRetiredEvent(VehicleId vehicleId)
    {
        VehicleId = vehicleId;
    }
}
