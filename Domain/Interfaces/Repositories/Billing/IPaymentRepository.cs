using Domain.Billing;
using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories.Billing
{
    public interface IPaymentRepository:IGenericRepository<Payment>
    {
        Task Update(Payment payment);
    }
}
