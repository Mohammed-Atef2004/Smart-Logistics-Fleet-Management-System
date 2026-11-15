using Domain.SharedKernel;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles.Events;

/// <summary>
/// Raised when a new vehicle joins the fleet.
/// Payload properties are PascalCase and never named "Id" so they can
/// never shadow <see cref="DomainEvent.Id"/> (the CS0108 class of bugs
/// where handlers logged the base event GUID instead of the vehicle id).
/// </summary>
public record VehicleRegisteredEvent : DomainEvent
{
    public VehicleId VehicleId { get; init; }

    public VehicleRegisteredEvent(VehicleId vehicleId)
    {
        VehicleId = vehicleId;
    }
}
