using Domain.Invoices.ValueObjects;

namespace Application.Features.Invoices.Commands.Create
{
    public record CreateInvoiceCommand():IRequest<Result<InvoiceId>>;
}