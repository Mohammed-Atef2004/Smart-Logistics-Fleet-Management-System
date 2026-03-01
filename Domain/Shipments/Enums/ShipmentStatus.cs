using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.Enums
{
    public enum ShipmentStatus
    {
        Draft = 0,
        Created = 1,
        ReadyForPickup = 2,
        InTransit = 3,
        AtCustoms = 4,
        OutForDelivery = 5,
        Delivered = 6,
        Failed = 7,
        Cancelled = 8,
        Returned = 9
    }

}
