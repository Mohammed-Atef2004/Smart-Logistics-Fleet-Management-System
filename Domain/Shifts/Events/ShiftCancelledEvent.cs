using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

namespace Domain.Shifts.Events
{
    public record ShiftCancelledEvent : DomainEvent
    {
        public ShiftId Id;

        public ShiftCancelledEvent(ShiftId id)
        {
            Id = id;
        }
    }
}