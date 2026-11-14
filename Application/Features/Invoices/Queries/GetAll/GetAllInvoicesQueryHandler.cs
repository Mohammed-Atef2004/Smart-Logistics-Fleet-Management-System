using Application.Features.Invoices.Queries.GetById;
using Domain.Invoices;

namespace Application.Features.Invoices.Queries.GetAll
{
    public sealed class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, List<GetInvoiceByIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceRepository _invoiceRepository;

        public GetAllInvoicesQueryHandler(IUnitOfWork unitOfWork, IInvoiceRepository invoiceRepository)
        {
            _unitOfWork = unitOfWork;
            _invoiceRepository = invoiceRepository;
        }

        public Task<List<GetInvoiceByIdResponse>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
        {
            var invoices = _invoiceRepository.EntityQuery.ToList();
            var response = invoices.Select(invoice => new GetInvoiceByIdResponse
            {
                Id = invoice.Id,
                Status = invoice.Status,
                Price = invoice.TotalPrice,
            }).ToList();
            return Task.FromResult(response);
        }
    }
}