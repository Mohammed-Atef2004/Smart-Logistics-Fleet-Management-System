using Application.Common.Interfaces;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Repositories.Shared;

/// <summary>
/// EF Core implementation of <see cref="IUnitOfWork"/>.
/// A thin transaction boundary over <see cref="AppDbContext"/>:
/// no repository accessors — repositories are registered in DI and
/// injected directly into handlers and domain services.
/// The DbContext is owned by the DI scope, so this class does not
/// dispose it.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.Database.CommitTransactionAsync(cancellationToken);
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.Database.RollbackTransactionAsync(cancellationToken);
    }
}
