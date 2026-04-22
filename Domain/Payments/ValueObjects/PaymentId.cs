using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Payments.ValueObjects
{
    public record PaymentId(Guid Value)
    {
        public static PaymentId New() => new(Guid.NewGuid());
        public static PaymentId FromGuid(Guid value) => new(value);
        public static PaymentId FromString(string value) => new(Guid.Parse(value));
        public override string ToString() => Value.ToString();
    }
}
