using Domain.Common;
using Domain.Vehicles.Enums;

public record VehicleStatusChangedEvent : DomainEvent
{
    public VehicleId Id { get; }
    public VehicleStatus NewStatus { get; }


    public VehicleStatusChangedEvent(VehicleId id, VehicleStatus newStatus)
    {
        Id = id;
        NewStatus = newStatus;
    }
}
