using Domain.Invoices;

namespace Application.Features.Invoices.Commands.AddItem
{
    public sealed class AddInvoiceItemCommandHandler : IRequestHandler<AddInvoiceItemCommand, Result>
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