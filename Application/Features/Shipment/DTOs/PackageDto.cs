namespace Application.Features.Shipment.DTOs;

/// <summary>Flat read model for a package inside a shipment.</summary>
public class PackageDto
{
    public Guid Id { get; set; }

    public string Description { get; set; } = default!;

    public decimal WeightKg { get; set; }

    public decimal DeclaredValue { get; set; }

    public string Currency { get; set; } = default!;

    public bool IsFragile { get; set; }

    public bool RequiresRefrigeration { get; set; }
}
