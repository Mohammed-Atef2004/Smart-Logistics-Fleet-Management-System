
using Domain.Claims;
using Domain.Claims.Enums;
using Domain.Claims.ValueObjects;
using Infrastructure.Presistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public sealed class ClaimRepository : IClaimRepository
{
    private readonly AppDbContext _context;

    public IQueryable<InsuranceClaim> EntityQuery => throw new NotImplementedException();

    public ClaimRepository(AppDbContext context) => _context = context;

    public async Task<InsuranceClaim?> GetByIdAsync(
        ClaimId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.insuranceClaims
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<InsuranceClaim?> GetByClaimNumberAsync(
        ClaimNumber claimNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.insuranceClaims
            .FirstOrDefaultAsync(c => c.ClaimNumber == claimNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<InsuranceClaim>> GetByShipmentIdAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.insuranceClaims
            .Where(c => c.ShipmentId == shipmentId)
            .OrderByDescending(c => c.SubmittedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InsuranceClaim>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.insuranceClaims
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.SubmittedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InsuranceClaim>> GetByStatusAsync(
        ClaimStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.insuranceClaims
            .Where(c => c.Status == status)
            .OrderBy(c => c.SubmittedAt)  // oldest first (FIFO review)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(InsuranceClaim claim, CancellationToken cancellationToken = default)
    {
        await _context.insuranceClaims.AddAsync(claim, cancellationToken);
    }

    public void Update(InsuranceClaim claim)
    {
        _context.insuranceClaims.Update(claim);
    }


    public Task AddAsync(InsuranceClaim entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(InsuranceClaim entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(Expression<Func<InsuranceClaim, bool>>? predicate = null)
    {
        throw new NotImplementedException();
    }
}
