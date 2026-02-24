using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

namespace Domain.Shifts.Events
{
    public record ShiftCompletedEvent : DomainEvent
    {
        public ShiftId Id;

        public ShiftCompletedEvent(ShiftId id)
        {
            Id = id;
        }
    }
}