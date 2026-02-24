using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

namespace Domain.Shifts.Events
{
    public record ShiftStartedEvent : DomainEvent
    {
        public ShiftId Id;

        public ShiftStartedEvent(ShiftId id)
        {
            Id = id;
        }
    }
}