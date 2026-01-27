using Domain.Common.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Domain.Shipment.ValueObjects
{

    [Owned]
    public class Route : ValueObject
    {
        public Address Origin { get; private set; }
        public Address Destination { get; private set; }
        public decimal EstimatedDistance { get; private set; } // in KM

        private Route() { } // EF Core

        public Route(Address origin, Address destination, decimal estimatedDistance)
        {
            Origin = origin ?? throw new ArgumentNullException(nameof(origin));
            Destination = destination ?? throw new ArgumentNullException(nameof(destination));

            if (estimatedDistance <= 0)
                throw new ArgumentException("Distance must be positive", nameof(estimatedDistance));

            EstimatedDistance = estimatedDistance;
        }

        public bool IsInternational() => Origin.Country != Destination.Country;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Origin;
            yield return Destination;
            yield return EstimatedDistance;
        }
    }
}
