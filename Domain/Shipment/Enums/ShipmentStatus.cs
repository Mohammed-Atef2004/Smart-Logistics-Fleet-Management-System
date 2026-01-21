using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Enums
{

    public enum ShipmentStatus
    {
        Created = 1,
        Assigned = 2,
        InTransit = 3,
        Delivered = 4,
        Cancelled = 5
    }

}
