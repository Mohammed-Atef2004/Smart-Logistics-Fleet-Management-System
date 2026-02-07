using Domain.SharedKernel;
using Domain.Shipment.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.DTOs
{
    public class ShipmentDto
    {
        public string TrackingNumber { get;  set; }
        public ShipmentStatus Status { get; set; }
        public Route Route { get; set; }
    }
}
