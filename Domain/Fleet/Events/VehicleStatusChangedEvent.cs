using Domain.Common;
using Domain.Fleet.Enums;

public class VehicleStatusChangedEvent : DomainEvent
{
    public Guid VehicleId { get; }
    public VehicleStatus NewStatus { get; }

    public VehicleStatusChangedEvent(Guid vehicleId, VehicleStatus newStatus)
    {
        VehicleId = vehicleId;
        NewStatus = newStatus;
    }
}
