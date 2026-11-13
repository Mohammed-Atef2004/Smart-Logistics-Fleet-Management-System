using Domain.SharedKernel;
using Infrastructure.Presistence.Interceptors;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Architecture.Tests;

/// <summary>
/// Regression tests for <see cref="DomainEventInterceptor"/> (ROADMAP item C5):
/// domain events must be published ONLY AFTER a successful database commit â€”
/// a failed save must never trigger handler side effects.
/// </summary>
public class DomainEventInterceptorTests
{
    [Fact]
    public void SaveChanges_WithSuccessfulCommit_Should_PublishDomainEvents()
    {
        using var context = CreateContext();

        var aggregate = new TestAggregate(Guid.NewGuid(), "first");
        aggregate.RaiseEvent();
        context.Add(aggregate);
        context.SaveChanges();

        Assert.Single(TestDomainEventHandler.Published);
    }

    [Fact]
    public async Task SaveChangesAsync_WithSuccessfulCommit_Should_PublishDomainEvents()
    {
        using var context = CreateContext();

        var aggregate = new TestAggregate(Guid.NewGuid(), "first");
        aggregate.RaiseEvent();
        context.Add(aggregate);
        await context.SaveChangesAsync();

        Assert.Single(TestDomainEventHandler.Published);
    }

    [Fact]
    public async Task SaveChangesAsync_WithFailedCommit_Should_Not_PublishDomainEvents()
    {
        using var context = CreateContext();

        // Seed a row that will collide with the next insert on the unique index.
        context.Add(new TestAggregate(Guid.NewGuid(), "duplicate"));
        await context.SaveChangesAsync();

        // This aggregate raises a domain event, but its save violates the
        // unique index on Name -> the commit fails.
        var failing = new TestAggregate(Guid.NewGuid(), "duplicate");
        failing.RaiseEvent();
        context.Add(failing);

        await Assert.ThrowsAnyAsync<DbUpdateException>(() => context.SaveChangesAsync());

        // The commit failed -> no side effects may have run.
        Assert.Empty(TestDomainEventHandler.Published);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_ClearEventsFromAggregates()
    {
        using var context = CreateContext();

        var aggregate = new TestAggregate(Guid.NewGuid(), "cleared");
        aggregate.RaiseEvent();
        context.Add(aggregate);
        await context.SaveChangesAsync();

        Assert.Empty(aggregate.DomainEvents);
    }

    // ------------------------------------------------------------------

    private static TestDbContext CreateContext()
    {
        // xUnit does not reset static state between tests.
        TestDomainEventHandler.Published.Clear();

        // Fresh in-memory database per test.
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var services = new ServiceCollection();

        // MediatR 14's LicenseAccessor resolves ILoggerFactory on first use.
        services.AddLogging();
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<TestDomainEvent>());
        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        var interceptor = new DomainEventInterceptor(
            mediator, NullLogger<DomainEventInterceptor>.Instance);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .AddInterceptors(interceptor)
            .Options;

        var context = new TestDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}

// ------------------------------------------------------------------
// Test doubles (defined here so they never leak into production code).
// ------------------------------------------------------------------

public sealed record TestDomainEvent(string Name) : DomainEvent;

public sealed class TestAggregate : AggregateRoot<Guid>
{
    public string Name { get; set; } = string.Empty;

    public TestAggregate(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public void RaiseEvent()
        => AddDomainEvent(new TestDomainEvent(Name));
}

public sealed class TestDomainEventHandler : INotificationHandler<TestDomainEvent>
{
    public static readonly List<string> Published = new();

    public Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
    {
        Published.Add(notification.Name);
        return Task.CompletedTask;
    }
}

public sealed class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    public DbSet<TestAggregate> Aggregates => Set<TestAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique index on Name is the deterministic way to make a REAL
        // database commit fail inside the tests.

        // DomainEvents are in-memory notifications, never persisted.
        modelBuilder.Ignore<DomainEvent>();
        modelBuilder.Entity<TestAggregate>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Name).IsUnique();
        });
    }
}
