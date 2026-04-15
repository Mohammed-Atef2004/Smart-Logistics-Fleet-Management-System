using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Invoices.ValueObjects
{
    public sealed record InvoiceId(Guid Id)
    {
        public static InvoiceId New() => new(Guid.NewGuid());
        public static InvoiceId From(string Id) => new(Guid.Parse(Id));
        public static InvoiceId From(Guid Id) => new(Id);

    }
}
