using Application.Common.Interfaces;
using Domain.Invoices;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class InvoiceRepository:GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(AppDbContext context) : base(context)
        {
        }

        // Interface signature (no predicate) differs from GenericRepository<T>.CountAsync(predicate)
        public Task<int> CountAsync() => _context.Invoices.CountAsync();
    }
}
