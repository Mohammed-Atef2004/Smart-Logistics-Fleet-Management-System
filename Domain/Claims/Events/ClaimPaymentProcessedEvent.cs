using Domain.Claims.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Claims.Events;

public record ClaimPaymentProcessedEvent(
    ClaimId id,
    Guid CustomerId,
    ClaimAmount PaidAmount) : DomainEvent;
