using Application.Features.Invoices.Queries.GetById;

namespace Application.Features.Invoices.Queries.GetAll
{
    public record GetAllInvoicesQuery: IRequest<List<GetInvoiceByIdResponse>>;
}