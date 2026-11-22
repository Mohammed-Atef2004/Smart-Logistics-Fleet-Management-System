using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Drivers.Events;

/// <summary>
/// Raised when a driver leaves the company. Termination is terminal:
/// a terminated driver can never be reactivated.
/// </summary>
public record DriverTerminatedEvent : DomainEvent
{
    public DriverId DriverId { get; init; }

    public DriverTerminatedEvent(DriverId driverId)
    {
        DriverId = driverId;
    }
}
