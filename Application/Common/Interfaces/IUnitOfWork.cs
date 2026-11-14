namespace Application.Common.Interfaces;

/// <summary>
/// Transaction-scoped unit of work owned by the Application layer and
/// implemented by Infrastructure on top of the EF Core DbContext.
/// Pure transaction boundary: repositories are NOT exposed from here —
/// handlers inject the repository interfaces directly.
/// Lifetime is the DI scope; the container owns the DbContext, so this
/// type is not IDisposable.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all pending changes in one atomic commit and returns the
    /// number of affected state entries.
    /// </summary>
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);

    /// <summary>Begins an explicit database transaction.</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Commits the current explicit database transaction.</summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Rolls back the current explicit database transaction.</summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
