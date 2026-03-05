using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.DTOs
{
    public class ShipmentDetailsDto
    {
        public Guid Id { get; set; }

        public string TrackingNumber { get; set; } = default!;

        public string Status { get; set; } = default!;

        public string SenderId { get; set; } = default!;

        public string? RecipientName { get; set; }

        public string? RecipientPhone { get; set; }

        public string DestinationCity { get; set; } = default!;

        public string? CarrierName { get; set; }

        public int TotalPackages { get; set; }

        public decimal TotalWeightKg { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<PackageDto> Packages { get; set; } = new();
    }
}
