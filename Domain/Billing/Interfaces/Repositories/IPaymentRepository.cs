using Domain.Billing.Entities;
using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Billing.Interfaces.Repositories
{
    public interface IPaymentRepository:IGenericRepository<Payment>
    {
        Task Update(Payment payment);
    }
}
