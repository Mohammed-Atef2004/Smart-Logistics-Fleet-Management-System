using Amazon.Runtime.Internal;
using Domain.Invoices;
using Domain.Invoices.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Commands.AddItem
{
    public record AddInvoiceItemCommand
        (InvoiceId invoiceId,
        string description,
        decimal price,
        int quantity):IRequest<Result>;
}
