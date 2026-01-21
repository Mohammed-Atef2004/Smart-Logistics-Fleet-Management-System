using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Entities
{
    public class TrackingUpdate : BaseEntity
    {
        public Guid ShipmentId { get; private set; }
        public string Location { get; private set; }
        public DateTime Timestamp { get; private set; }

        private TrackingUpdate() { }

        public TrackingUpdate(Guid shipmentId, string location)
        {
            ShipmentId = shipmentId;
            Location = location;
            Timestamp = DateTime.UtcNow;
        }
    }

}
