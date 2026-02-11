using Domain.Common;

public record VehicleRegisteredEvent : DomainEvent
{
    private VehicleId id;

    public VehicleRegisteredEvent(VehicleId id)
    {
        this.id = id;
    }
}