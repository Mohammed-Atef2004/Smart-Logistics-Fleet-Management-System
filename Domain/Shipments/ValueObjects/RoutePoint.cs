using Domain.SharedKernel;
using Domain.Shipments.Enums;

namespace Domain.Shipments.ValueObjects;

/// <summary>
/// A point the shipment passed through. The explicit <see cref="Id"/> gives
/// every point a stable database identity, which removes the shadow key the
/// EF mapping used to generate (and which could collide when several points
/// were added in the same unit of work).
/// </summary>
public sealed record RoutePoint
{
    public Guid Id { get; private set; }
    public string Location { get; init; } = default!;
    public string Description { get; init; } = default!;
    public DateTime ArrivedAt { get; init; }
    public RoutePointType Type { get; init; }

    private RoutePoint() { } // EF Core

    private RoutePoint(string location, string description, DateTime arrivedAt, RoutePointType type)
    {
        Id = Guid.NewGuid();
        Location = location;
        Description = description;
        ArrivedAt = arrivedAt;
        Type = type;
    }

    public static Result<RoutePoint> Create(
        string location,
        string description,
        DateTime arrivedAt,
        RoutePointType type)
    {
        if (string.IsNullOrWhiteSpace(location))
            return Result<RoutePoint>.Failure(new Error(
                "RoutePoint.EmptyLocation",
                "Route point location cannot be empty."));
        if (string.IsNullOrWhiteSpace(description))
            return Result<RoutePoint>.Failure(new Error(
                "RoutePoint.EmptyDescription",
                "Route point description cannot be empty."));
        if (arrivedAt == default)
            return Result<RoutePoint>.Failure(new Error(
                "RoutePoint.InvalidArrivedAt",
                "Route point arrival time is required."));

        return Result<RoutePoint>.Success(new RoutePoint(
            location.Trim(),
            description.Trim(),
            NormalizeToUtc(arrivedAt),
            type));
    }

    public bool IsOrigin => Type == RoutePointType.Origin;
    public bool IsFinalDestination => Type == RoutePointType.Destination;

    private static DateTime NormalizeToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
