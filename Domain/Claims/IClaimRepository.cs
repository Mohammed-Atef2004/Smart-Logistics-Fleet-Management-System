using Domain.Claims.ValueObjects;

namespace Domain.Claims;

/// <summary>
/// Repository contract for the InsuranceClaim aggregate.
/// Owned by the Domain (classic DDD); implemented by Infrastructure.
/// </summary>
public interface IClaimRepository
{
    /// <summary>Deferred query root for read-side composition.</summary>
    IQueryable<InsuranceClaim> EntityQuery { get; }

    Task AddAsync(InsuranceClaim entity);
}
