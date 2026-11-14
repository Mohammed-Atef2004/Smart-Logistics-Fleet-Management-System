using Domain.Invoices.ValueObjects;

namespace Application.Features.Invoices.Queries.GetById
{
    public record GetInvoiceByIdQuery(InvoiceId invoiceId):IRequest<GetInvoiceByIdResponse>;
}