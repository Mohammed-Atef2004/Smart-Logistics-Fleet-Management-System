using Domain.Invoices.ValueObjects;

namespace Application.Features.Invoices.Commands.Issue
{
    public record IssueInvoiceCommand
        (InvoiceId invoiceId):IRequest<Result>;
}