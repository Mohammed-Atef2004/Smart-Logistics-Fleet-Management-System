using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Invoices
{
    public class InvoiceItem
    {
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public InvoiceItem() { }
        public InvoiceItem(string description, decimal price, int quantity)
        {
            Description = description;
            Price = price;
            Quantity = quantity;
        }

    }
}
