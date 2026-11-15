using Application.Common.Interfaces;

namespace Application.Tests.Fakes;

/// <summary>
/// In-memory IUnitOfWork that counts CompleteAsync calls and exposes the
/// fake repositories registered for the test scope.
/// </summary>
public sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }
    public FakeUserRepository Users { get; } = new();

    public Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.FromResult(0);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
