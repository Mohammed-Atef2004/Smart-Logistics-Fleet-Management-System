using Domain.Claims.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Claims.Events;

public record ClaimSubmittedEvent(
    ClaimId id,
    Guid ShipmentId,
    Guid CustomerId,
    ClaimAmount Amount) : DomainEvent;
