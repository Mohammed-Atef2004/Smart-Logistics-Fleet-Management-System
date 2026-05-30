using Domain.Payments;
using Domain.Payments.Enums;
using Domain.Payments.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Presistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    // =====================
    // Queryable source
    // =====================
    public IQueryable<Payment> Query => _context.Payments;

    // =====================
    // Get By Id
    // =====================
    public async Task<Payment?> GetByIdAsync(
        PaymentId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    // =====================
    // Get By Invoice
    // =====================
    public async Task<Payment?> GetByInvoiceIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.InvoiceId.Id == invoiceId, cancellationToken);
    }

    // =====================
    // Get By Status
    // =====================
    public async Task<List<Payment>> GetByStatusAsync(
        PaymentStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);
    }

    // =====================
    // Add
    // =====================
    public async Task AddAsync(
        Payment payment,
        CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
    }

    // =====================
    // Update
    // =====================
    public void Update(Payment payment)
    {
        _context.Payments.Update(payment);
    }

    // =====================
    // Delete
    // =====================
    public void Delete(Payment payment)
    {
        _context.Payments.Remove(payment);
    }
}