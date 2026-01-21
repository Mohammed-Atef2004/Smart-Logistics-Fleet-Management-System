using Domain.Billing.Events;
using Domain.Billing.Rules;
using Domain.Billing.ValueObjects;
using Domain.Common;

namespace Domain.Billing.Entities
{
    public class Invoice : AggregateRoot
    {
        public Guid ShipmentId { get; private set; }
        public Money TotalAmount { get; private set; }
        public bool IsPaid { get; private set; }

        private Invoice() { }

        public Invoice(Guid shipmentId, Money totalAmount)
        {
            CheckRule(new ShipmentIdRequiredRule(shipmentId));
            CheckRule(new InvoiceTotalAmountRequiredRule(totalAmount));

            ShipmentId = shipmentId;
            TotalAmount = totalAmount;
            IsPaid = false;
        }

        public void MarkAsPaid()
        {
            CheckRule(new InvoiceMustNotBePaidTwiceRule(IsPaid));

            IsPaid = true;
            AddDomainEvent(new InvoicePaidEvent(Id));
        }
    }
}
