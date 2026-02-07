using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Invoices
{
    public interface IInvoiceRepository:IGenericRepository<Invoice>
    {
        Task Update(Invoice invoice);
    }
}
