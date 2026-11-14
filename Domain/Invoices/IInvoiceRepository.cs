namespace Domain.Invoices;

/// <summary>
/// Repository contract for the Invoice aggregate.
/// Owned by the Domain (classic DDD); implemented by Infrastructure.
/// </summary>
public interface IInvoiceRepository
{
    /// <summary>Deferred query root for read-side composition.</summary>
    IQueryable<Invoice> EntityQuery { get; }

    Task AddAsync(Invoice entity);
    void Update(Invoice entity);

    Task<int> CountAsync();
}
