using Domain.Common;
using Domain.Shipment.Enums;

namespace Domain.Shipment.Entities
{

    public class Package : BaseEntity
    {
        public Guid ShipmentId { get; private set; }
        public double Weight { get; private set; }
        public string Description { get; private set; }
        public PackageType Type { get; private set; }
        public decimal DeclaredValue { get; private set; }

        private Package() { } // EF Core

        internal Package(Guid shipmentId, double weight, string description, PackageType type, decimal declaredValue)
        {
            if (weight <= 0)
                throw new ArgumentException("Package weight must be positive", nameof(weight));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description));
            if (declaredValue < 0)
                throw new ArgumentException("Declared value cannot be negative", nameof(declaredValue));

            ShipmentId = shipmentId;
            Weight = weight;
            Description = description;
            Type = type;
            DeclaredValue = declaredValue;
        }

        public bool RequiresSpecialHandling() => Type == PackageType.Fragile ||
                                                   Type == PackageType.Perishable ||
                                                   Type == PackageType.Hazardous;
    }
}