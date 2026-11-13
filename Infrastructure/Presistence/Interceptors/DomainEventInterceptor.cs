using System.Collections.Concurrent;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Presistence.Interceptors;

/// <summary>
/// Collects domain events from tracked aggregate roots and publishes them via MediatR
/// <b>after</b> the database commit succeeds.
///
/// Flow:
///   SavingChanges       -> collect events from tracked aggregates, clear them from the
///                          aggregates, and stash them per-context. NO publishing here.
///   SavedChanges        -> called only after a successful commit: pop the stashed
///                          events (first, so nested SaveChanges calls made by event
///                          handlers get their own list) and publish them.
///   SaveChangesFailed   -> the commit failed: discard the stashed events so no side
///                          effects ever run for a transaction that did not persist.
///
/// Known limitation (deliberate): if the application crashes between the commit and
/// the dispatch, the events are lost. The Outbox pattern (tracked in ROADMAP.md) is
/// the follow-up fix for guaranteed delivery; until then, exceptions thrown by a
/// handler after a successful commit propagate to the caller.
/// </summary>
public sealed class DomainEventInterceptor : SaveChangesInterceptor
{

    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventInterceptor> _logger;

    // Stashed per-context so a nested SaveChanges (e.g. an event handler persisting
    // cross-aggregate state during post-commit dispatch) never clobbers the outer
    // save's pending event list.
    private readonly ConcurrentDictionary<DbContext, List<DomainEvent>> _pendingEvents = new();

    public DomainEventInterceptor(IMediator mediator, ILogger<DomainEventInterceptor> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // ------------------------------------------------------------------
    // 1) Before the commit: collect + clear + stash. Never publish here.
    // ------------------------------------------------------------------

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
                CollectEvents(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
                CollectEvents(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    // ------------------------------------------------------------------
    // 2) After a successful commit: pop the stash, then publish.
    // ------------------------------------------------------------------

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        PublishPending(eventData.Context);
        return result;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await PublishPendingAsync(eventData.Context, cancellationToken);
        return result;
    }

    // ------------------------------------------------------------------
    // 3) The commit failed: discard the events. No side effects.
    // ------------------------------------------------------------------

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        DiscardPending(eventData.Context);
    }

    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        DiscardPending(eventData.Context);
        return Task.CompletedTask;
    }

    // ------------------------------------------------------------------

    private void CollectEvents(DbContext? context)
    {
        if (context == null)
            return;

        var entities = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        if (entities.Count == 0)
            return;

        var events = entities.SelectMany(e => e.DomainEvents).ToList();

        // Clear from the aggregates first so a nested SaveChanges cannot
        // collect the same events again.
        foreach (var entity in entities)
            entity.ClearDomainEvents();

        var stash = _pendingEvents.GetOrAdd(context, _ => new List<DomainEvent>());
        stash.AddRange(events);
    }

    private async Task PublishPendingAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context == null || !_pendingEvents.TryRemove(context, out var events) || events.Count == 0)
            return;

        // The stash entry is already removed, so nested SaveChanges calls made
        // by the handlers below start from a clean list for this context.
        foreach (var domainEvent in events)
        {
            _logger.LogInformation("Publishing domain event {EventName} ({EventId}) after commit",
                domainEvent.GetType().Name, domainEvent.Id);

            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }

    private void PublishPending(DbContext? context)
    {
        if (context == null || !_pendingEvents.TryRemove(context, out var events) || events.Count == 0)
            return;

        foreach (var domainEvent in events)
        {
            _logger.LogInformation("Publishing domain event {EventName} ({EventId}) after commit",
                domainEvent.GetType().Name, domainEvent.Id);

            _mediator.Publish(domainEvent).GetAwaiter().GetResult();
        }
    }

    private void DiscardPending(DbContext? context)
    {
        if (context != null && _pendingEvents.TryRemove(context, out var events) && events.Count > 0)
        {
            _logger.LogWarning(
                "Save failed for {Context}; discarded {Count} domain event(s) so no side effects run for a failed commit",
                context.GetType().Name, events.Count);
        }
    }
}
