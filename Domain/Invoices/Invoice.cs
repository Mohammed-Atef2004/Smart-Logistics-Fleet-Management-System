using Domain.Invoices.ValueObjects;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Invoices.Rules;
using Domain.Invoices.Events;

namespace Domain.Invoices
{
    public class Invoice:AggregateRoot<InvoiceId>
    {
        public double Price { get; private set; }
        public bool IsPaid { get; private set; }
        private Invoice() { }
        public static Result<Invoice> Create(double price)
        {
            var invoice = new Invoice();
            var PriceCheck =invoice.CheckRule(new PriceMustBePositiveRule(price));
            if (PriceCheck.IsFailure)
                return Result<Invoice>.Failure(PriceCheck.Error);
            
            invoice.AddDomainEvent(new InvoiceCreatedEvent(invoice.Id, invoice.Price));
            return Result<Invoice>.Success(invoice);


        }
    }
}
