using Domain.Claims.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Claims.Events;

public record ClaimClosedEvent(
    ClaimId id) : DomainEvent;
