using Amazon.Runtime.Internal;
using Domain.Invoices.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Commands.Issue
{
    public record IssueInvoiceCommand
        (InvoiceId invoiceId):IRequest<Result>;
}
