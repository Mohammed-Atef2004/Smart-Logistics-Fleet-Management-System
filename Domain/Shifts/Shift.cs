using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts.Enums;
using Domain.Shifts.Errors;
using Domain.Shifts.Events;
using Domain.Shifts.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shifts
{
    public sealed class Shift : AggregateRoot<ShiftId>
    {
        public DriverId DriverId { get; private set; }
        public DateTime ShiftStart { get; private set; }
        public DateTime ShiftEnd { get; private set; }
        public ShiftStatus Status { get; private set; }

        private Shift() { }

        private Shift(ShiftId id, DriverId driverId, DateTime start, DateTime end)
        {
            Id = id;
            DriverId = driverId;
            ShiftStart = start;
            ShiftEnd = end;
            Status = ShiftStatus.Planned;
        }

        public static Result<Shift> Create(DriverId driverId, DateTime start, DateTime end)
        {
            if (start >= end) return Result<Shift>.Failure(new("Shift.InvalidDuration", "Start must be before End"));
            var shift = new Shift(new ShiftId(Guid.NewGuid()), driverId, start, end);
            shift.AddDomainEvent(new ShiftCreatedEvent(shift.Id));
            return Result<Shift>.Success(shift);
        }

        public Result Start()
        {
            if (Status != ShiftStatus.Planned) return Result.Failure(new("Shift.InvalidState", "Cannot start"));
            Status = ShiftStatus.Active;
            AddDomainEvent(new ShiftStartedEvent(Id));
            return Result.Success();
        }

        public Result Complete()
        {
            if (Status != ShiftStatus.Active) return Result.Failure(new("Shift.InvalidState", "Cannot complete"));
            Status = ShiftStatus.Completed;
            AddDomainEvent(new ShiftCompletedEvent(Id));
            return Result.Success();
        }

        public Result Cancel()
        {
            if (Status == ShiftStatus.Completed) return Result.Failure(new("Shift.Completed", "Cannot cancel completed shift"));
            Status = ShiftStatus.Cancelled;
            AddDomainEvent(new ShiftCancelledEvent(Id));
            return Result.Success();
        }
    }

    public enum ShiftStatus { Planned, Active, Completed, Cancelled, Aborted }
}
