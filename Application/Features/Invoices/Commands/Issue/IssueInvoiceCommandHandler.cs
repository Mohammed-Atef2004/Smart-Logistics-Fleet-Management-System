using Domain.Interfaces.Repositories;
using Domain.Invoices;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Commands.Issue
{
    public class IssueInvoiceCommandHandler : IRequestHandler<IssueInvoiceCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceRepository _invoiceRepository;

        public IssueInvoiceCommandHandler(IUnitOfWork unitOfWork, IInvoiceRepository invoiceRepository)
        {
            _unitOfWork = unitOfWork;
            _invoiceRepository = invoiceRepository;
        }

        public Task<Result> Handle(IssueInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = _invoiceRepository.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.invoiceId).Result;
            if (invoice == null)
            {
                return Task.FromResult(Result.Failure(InvoiceErrors.NotFound));
            }   
            invoice.Issue();
            _invoiceRepository.Update(invoice);
            _unitOfWork.CompleteAsync(cancellationToken);
            return Task.FromResult(Result.Success());
        }
    }
}
