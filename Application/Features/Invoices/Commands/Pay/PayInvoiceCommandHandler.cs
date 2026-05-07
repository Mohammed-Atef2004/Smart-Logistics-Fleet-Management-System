using Domain.Interfaces.Repositories;
using Domain.Invoices;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Commands.Pay
{
    public class PayInvoiceCommandHandler : IRequestHandler<PayInvoiceCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceRepository _invoiceRepository;

        public PayInvoiceCommandHandler(IUnitOfWork unitOfWork, IInvoiceRepository invoiceRepository)
        {
            _unitOfWork = unitOfWork;
            _invoiceRepository = invoiceRepository;
        }

        public Task<Result> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = _invoiceRepository.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.invoiceId).Result;
            if (invoice == null)
            {
                return Task.FromResult(Result.Failure(InvoiceErrors.NotFound));
            }
            invoice.Pay();
            _invoiceRepository.Update(invoice);
            _unitOfWork.CompleteAsync(cancellationToken);
            return Task.FromResult(Result.Success());
        }
    }
}
