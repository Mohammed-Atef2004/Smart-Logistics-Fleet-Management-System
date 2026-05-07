using Domain.Interfaces.Repositories;
using Domain.Invoices;
using Domain.Invoices.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Commands.Create
{
    public class CreateInvoiceCommandHandler:IRequestHandler<CreateInvoiceCommand, Result<InvoiceId>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceRepository _invoiceRepository;

        public CreateInvoiceCommandHandler(IUnitOfWork unitOfWork, IInvoiceRepository invoiceRepository)
        {
            _unitOfWork = unitOfWork;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Result<InvoiceId>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoiceResult = Invoice.Create();
            if (invoiceResult.IsFailure)
                return Result<InvoiceId>.Failure(invoiceResult.Error);
            var invoice = invoiceResult.Value;
            await _invoiceRepository.AddAsync(invoice);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result<InvoiceId>.Success(invoice.Id);
        }
    }
}
