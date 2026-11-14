using Domain.Invoices.ValueObjects;

namespace Application.Features.Invoices.Commands.Pay
{
    public record PayInvoiceCommand
    (
       InvoiceId invoiceId
    ): IRequest<Result>;
}