using Domain.SharedKernel;
using Domain.Vehicles.Enums;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles.Events;

/// <summary>
/// Raised whenever the vehicle status machine accepts a transition.
/// The payload property is "VehicleId", not "Id": the old name hid the
/// base <see cref="DomainEvent.Id"/> (CS0108) and handlers resolved the
/// wrong value.
/// </summary>
public record VehicleStatusChangedEvent : DomainEvent
{
    public VehicleId VehicleId { get; init; }
    public VehicleStatus NewStatus { get; init; }

    public VehicleStatusChangedEvent(VehicleId vehicleId, VehicleStatus newStatus)
    {
        VehicleId = vehicleId;
        NewStatus = newStatus;
    }
}
