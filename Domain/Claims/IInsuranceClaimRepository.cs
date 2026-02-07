using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Claims
{
    public interface IInsuranceClaimRepository:IGenericRepository<InsuranceClaim>
    {
        Task Update(InsuranceClaim insuranceClaim);
    }
}
