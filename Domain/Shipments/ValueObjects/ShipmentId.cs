using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipments.ValueObjects
{
    public sealed record ShipmentId(Guid Value)
    {
        public static ShipmentId New() => new(Guid.NewGuid());
        public static ShipmentId From(Guid value) => new(value);
        public static ShipmentId From(string value) => new(Guid.Parse(value));
        public override string ToString() => Value.ToString();
    }
}
