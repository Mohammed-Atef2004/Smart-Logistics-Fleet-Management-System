namespace Application.Features.Shipment.DTOs
{
    public class PackageDto
    {
        public Guid Id { get; set; }

        public string Description { get; set; } = default!;

        public decimal WeightKg { get; set; }

        public decimal DeclaredValue { get; set; }

        public bool IsFragile { get; set; }
    }
}