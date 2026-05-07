using Domain.Interfaces.Repositories;
using Domain.Invoices;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Queries.GetById
{
    public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, GetInvoiceByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoiceByIdQueryHandler(IUnitOfWork unitOfWork, IInvoiceRepository invoiceRepository)
        {
            _unitOfWork = unitOfWork;
            _invoiceRepository = invoiceRepository;
        }

        public  Task<GetInvoiceByIdResponse> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var invoice = _invoiceRepository.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.invoiceId).Result;
            if (invoice == null)
            {
                return  Task.FromResult<GetInvoiceByIdResponse>(null);
            }
            var response = new GetInvoiceByIdResponse
            {
                Id = invoice.Id,
                Status = invoice.Status,
                Price = invoice.TotalPrice,
            };
            return  Task.FromResult(response);
        }
    }
}
