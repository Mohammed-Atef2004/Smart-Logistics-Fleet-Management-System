using Domain.Invoices.Enums;
using Domain.Invoices.ValueObjects;

namespace Application.Features.Invoices.Queries.GetById
{
    public class GetInvoiceByIdResponse
    {
        public InvoiceId Id { get; set; }
        public decimal Price { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}