using Domain.Invoices.ValueObjects;

namespace Application.Features.Invoices.Commands.Cancel
{
    public record CancelInvoiceCommand
    (InvoiceId invoiceId): IRequest<Result>;
}