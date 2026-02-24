using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

namespace Domain.Shifts.Events
{
    public record ShiftCreatedEvent : DomainEvent
    {
        private ShiftId Id;

        public ShiftCreatedEvent(ShiftId id)
        {
            Id = id;
        }
    }
}