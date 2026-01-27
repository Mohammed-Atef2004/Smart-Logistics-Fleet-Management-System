using Domain.Common;
using Microsoft.EntityFrameworkCore;
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
        public string? Notes { get; private set; }
        public DateTime Timestamp { get; private set; }

        private TrackingUpdate() { } // EF Core

        internal TrackingUpdate(Guid shipmentId, string location, string? notes = null)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location is required", nameof(location));

            ShipmentId = shipmentId;
            Location = location;
            Notes = notes;
            Timestamp = DateTime.UtcNow;
        }
    }

}
