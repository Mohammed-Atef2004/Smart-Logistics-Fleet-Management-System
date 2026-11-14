using Domain.Invoices;
using Domain.Invoices.ValueObjects;

namespace Application.Features.Invoices.Commands.AddItem
{
    public record AddInvoiceItemCommand
        (InvoiceId invoiceId,
        string description,
        decimal price,
        int quantity):IRequest<Result>;
}