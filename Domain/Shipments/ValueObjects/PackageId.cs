using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.ValueObjects
{
    public sealed record PackageId(Guid Value)
    {
        public static PackageId New() => new(Guid.NewGuid());
        public static PackageId From(Guid value) => new(value);
        public override string ToString() => Value.ToString();
    }
}
