using Application.Features.Invoices.Queries.GetById;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Queries.GetAll
{
    public record GetAllInvoicesQuery: IRequest<List<GetInvoiceByIdResponse>>;
}
