using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.DTOs
{
    public class ShipmentPackageDto
    {
        public double Weight { get; set; }
        public string Description { get; set; }
        public int packageType { get; set; }
       public decimal DeclaredValue { get; set; }
    }
}
