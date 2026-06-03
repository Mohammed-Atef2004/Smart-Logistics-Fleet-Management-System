using Domain.Claims.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Claims.Events;

public record ClaimRejectedEvent(
    ClaimId id,
    Guid CustomerId,
    string Reason) : DomainEvent;
