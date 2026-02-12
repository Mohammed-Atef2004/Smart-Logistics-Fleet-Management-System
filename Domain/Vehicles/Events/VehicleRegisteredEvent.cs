using Domain.SharedKernel;

public record VehicleRegisteredEvent : DomainEvent
{
    public VehicleId id { get; init; }

    public VehicleRegisteredEvent(VehicleId id)
    {
        this.id = id;
    }
}