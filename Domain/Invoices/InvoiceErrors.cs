using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Invoices
{
    public static class InvoiceErrors
    {
        public static Error NotDraft =>
            new("Invoice.NotDraft", "Cannot modify a non-draft invoice");

        public static Error Empty =>
            new("Invoice.Empty", "Invoice must have at least one item");

        public static Error Paid =>
            new("Invoice.Paid", "Invoice is already paid");
        public static Error NotIssued =>
            new("Invoice.NotIssued", "Only issued invoices can be paid");
    }
}
