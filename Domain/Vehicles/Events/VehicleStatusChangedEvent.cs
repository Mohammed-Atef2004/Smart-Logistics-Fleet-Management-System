using Domain.SharedKernel;
using Domain.Vehicles.Enums;

public record VehicleStatusChangedEvent : DomainEvent
{
    public VehicleId Id { get; init; }
    public VehicleStatus NewStatus { get; init; }


    public VehicleStatusChangedEvent(VehicleId id, VehicleStatus newStatus)
    {
        Id = id;
        NewStatus = newStatus;
    }
}
