using Domain.SharedKernel;
using Domain.Shipments.Enums;

namespace Domain.Shipments.ValueObjects;

/// <summary>
/// Tracking state of a shipment. The status changes are functional updates:
/// every transition returns a new value object and the aggregate keeps the
/// latest one.
/// </summary>
public sealed record TrackingInfo
{
    public string TrackingNumber { get; init; } = default!;
    public ShipmentStatus Status { get; init; }
    public string StatusDescription { get; init; } = default!;
    public DateTime UpdatedAt { get; init; }
    public string? CarrierName { get; init; }
    public DateTime? EstimatedDeliveryDate { get; init; }

    private TrackingInfo() { } // EF Core

    private TrackingInfo(
        string trackingNumber,
        ShipmentStatus status,
        string statusDescription,
        DateTime updatedAt,
        string? carrierName,
        DateTime? estimatedDeliveryDate)
    {
        TrackingNumber = trackingNumber;
        Status = status;
        StatusDescription = statusDescription;
        UpdatedAt = updatedAt;
        CarrierName = carrierName;
        EstimatedDeliveryDate = estimatedDeliveryDate;
    }

    public static Result<TrackingInfo> Create(
        string trackingNumber,
        ShipmentStatus status,
        string statusDescription,
        string? carrierName = null,
        DateTime? estimatedDeliveryDate = null)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            return Result<TrackingInfo>.Failure(new Error(
                "TrackingInfo.EmptyTrackingNumber",
                "Tracking number cannot be empty."));
        if (string.IsNullOrWhiteSpace(statusDescription))
            return Result<TrackingInfo>.Failure(new Error(
                "TrackingInfo.EmptyStatusDescription",
                "Status description cannot be empty."));

        return Result<TrackingInfo>.Success(new TrackingInfo(
            trackingNumber.Trim(),
            status,
            statusDescription.Trim(),
            DateTime.UtcNow,
            string.IsNullOrWhiteSpace(carrierName) ? null : carrierName.Trim(),
            estimatedDeliveryDate));
    }

    /// <summary>Functional update - returns a new value object with the new status.</summary>
    public TrackingInfo WithStatus(ShipmentStatus newStatus, string description) =>
        this with
        {
            Status = newStatus,
            StatusDescription = description,
            UpdatedAt = DateTime.UtcNow
        };

    public TrackingInfo WithCarrier(string carrierName, DateTime? estimatedDeliveryDate = null) =>
        this with
        {
            CarrierName = carrierName,
            EstimatedDeliveryDate = estimatedDeliveryDate,
            UpdatedAt = DateTime.UtcNow
        };

    public bool IsTerminal =>
        Status is ShipmentStatus.Delivered or ShipmentStatus.Cancelled or ShipmentStatus.Returned;
}
