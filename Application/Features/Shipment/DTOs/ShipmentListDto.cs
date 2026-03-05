using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.DTOs
{
    public class ShipmentListDto
    {
        public Guid Id { get; set; }

        public string TrackingNumber { get; set; } = default!;

        public string Status { get; set; } = default!;

        public string DestinationCity { get; set; } = default!;

        public string? CarrierName { get; set; }

        public int TotalPackages { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
