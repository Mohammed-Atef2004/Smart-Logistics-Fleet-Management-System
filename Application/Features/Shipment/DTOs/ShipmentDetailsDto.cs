namespace Application.Features.Shipment.DTOs;

/// <summary>Full read model for the shipment details endpoint.</summary>
public class ShipmentDetailsDto
{
    public Guid Id { get; set; }

    public string TrackingNumber { get; set; } = default!;

    public string Status { get; set; } = default!;

    public string StatusDescription { get; set; } = default!;

    public string SenderId { get; set; } = default!;

    public string? RecipientName { get; set; }

    public string? RecipientPhone { get; set; }

    public string DestinationCity { get; set; } = default!;

    public string DestinationAddress { get; set; } = default!;

    public string? CarrierName { get; set; }

    public DateTime? EstimatedDeliveryDate { get; set; }

    public int TotalPackages { get; set; }

    public decimal TotalWeightKg { get; set; }

    public decimal TotalDeclaredValue { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public List<PackageDto> Packages { get; set; } = [];
}
