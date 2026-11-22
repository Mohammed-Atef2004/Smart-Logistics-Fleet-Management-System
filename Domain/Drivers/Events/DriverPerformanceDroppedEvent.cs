using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Drivers.Events;

/// <summary>
/// Raised when the running average drops below the performance
/// threshold. The rating and sample size travel in the payload so
/// handlers do not have to re-query the aggregate.
/// </summary>
public record DriverPerformanceDroppedEvent : DomainEvent
{
    public DriverId DriverId { get; init; }
    public double Rating { get; init; }
    public int RatingCount { get; init; }

    public DriverPerformanceDroppedEvent(DriverId driverId, double rating, int ratingCount)
    {
        DriverId = driverId;
        Rating = rating;
        RatingCount = ratingCount;
    }
}
