using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Shipment
{
    public class Route : BaseEntity
    {
        public Address From { get; private set; }
        public Address To { get; private set; }

        // Private constructor للـ EF Core
        private Route() { }

        public Route(Address from, Address to)
        {
            From = from;
            To = to;
        }
    }
}