using Amazon.Runtime.Internal;
using Domain.Invoices.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Queries.GetById
{
    public record GetInvoiceByIdQuery(InvoiceId invoiceId):IRequest<GetInvoiceByIdResponse>;
}
