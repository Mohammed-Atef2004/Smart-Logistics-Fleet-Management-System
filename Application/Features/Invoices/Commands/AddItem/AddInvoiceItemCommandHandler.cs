using Domain.Interfaces.Repositories;
using Domain.Invoices;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Commands.AddItem
{
    public class AddInvoiceItemCommandHandler : IRequestHandler<AddInvoiceItemCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceRepository _invoiceRepository;

        public AddInvoiceItemCommandHandler(IUnitOfWork unitOfWork, IInvoiceRepository invoiceRepository)
        {
            _unitOfWork = unitOfWork;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Result> Handle(AddInvoiceItemCommand request, CancellationToken cancellationToken)
        {
            var invoiceResult = _invoiceRepository.EntityQuery.SingleOrDefault(x => x.Id == request.invoiceId);
            if (invoiceResult == null)
            {
                return Result.Failure(new Error("Invoice.NotFound", "Invoice not found"));
            }
            invoiceResult.AddItem(request.description, request.price, request.quantity);
            _invoiceRepository.Update(invoiceResult);
            await _invoiceRepository.CountAsync();
            return Result.Success();
        }
    }
}
