using Domain.Common;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Billing
{
    public class Invoice : AggregateRoot
    {
        public Guid ShipmentId { get; private set; }
        public Money TotalAmount { get; private set; }
        public bool IsPaid { get; private set; }

        private Invoice() { }

        public Invoice(Guid shipmentId, Money totalAmount)
        {
            ShipmentId = shipmentId;
            TotalAmount = totalAmount;
            IsPaid = false;
        }

        public void MarkAsPaid()
        {
            if (IsPaid)
                throw new DomainException("Invoice already paid.");

            IsPaid = true;
            AddDomainEvent(new InvoicePaidEvent(Id));
        }
    }
}
