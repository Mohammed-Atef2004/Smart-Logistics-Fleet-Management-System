using Domain.Claims.Enums;
using Domain.Claims.ValueObjects;
using Domain.Interfaces.Repositories;

namespace Domain.Claims;

public interface IClaimRepository : IGenericRepository<InsuranceClaim>
{
    
}
