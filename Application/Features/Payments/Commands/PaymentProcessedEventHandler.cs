using Application.Common.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Invoices;
using Domain.Invoices.ValueObjects;
using Domain.Payments.Events;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Payments.EventHandlers
{
    
    public class PaymentProcessedEventHandler : INotificationHandler<PaymentProcessedEvent>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentProcessedEventHandler(
            IInvoiceRepository invoiceRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(PaymentProcessedEvent notification, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.EntityQuery.FirstOrDefaultAsync(
                x=>x.Id==
                notification.InvoiceId,
                cancellationToken);

            if (invoice is null)
                return; // log it in production - مفروض ما يحصلش ده

            invoice.Pay();

            _invoiceRepository.Update(invoice);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}
