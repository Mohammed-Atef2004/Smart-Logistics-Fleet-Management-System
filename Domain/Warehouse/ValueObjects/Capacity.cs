using Domain.Common.Domain.Common;
using Domain.SharedKernel;
using Domain.Warehouse.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.ValueObjects
{
    public sealed class Capacity : ValueObject
    {
        public int MaxSlots { get; }
        public int UsedSlots { get; }
        public int AvailableSlots => MaxSlots - UsedSlots;
        public bool IsFull => AvailableSlots == 0;

        private Capacity() { }

        private Capacity(int maxSlots, int usedSlots)
        {
            MaxSlots = maxSlots;
            UsedSlots = usedSlots;
        }

        public static Result<Capacity> Create(int maxSlots, int usedSlots = 0)
        {
            if (maxSlots <= 0)
                return Result<Capacity>.Failure(CapacityErrors.InvalidMaxSlots);

            if (usedSlots < 0)
                return Result<Capacity>.Failure(CapacityErrors.NegativeUsedSlots);

            if (usedSlots > maxSlots)
                return Result<Capacity>.Failure(CapacityErrors.UsedExceedsMax);

            return Result<Capacity>.Success(new Capacity(maxSlots, usedSlots));
        }

        public Result<Capacity> Reserve(int slots = 1)
        {
            if (slots <= 0)
                return Result<Capacity>.Failure(CapacityErrors.InvalidSlotsCount);

            if (UsedSlots + slots > MaxSlots)
                return Result<Capacity>.Failure(CapacityErrors.InsufficientCapacity);

            return Result<Capacity>.Success(new Capacity(MaxSlots, UsedSlots + slots));
        }

        public Result<Capacity> Release(int slots = 1)
        {
            if (slots <= 0)
                return Result<Capacity>.Failure(CapacityErrors.InvalidSlotsCount);

            if (UsedSlots - slots < 0)
                return Result<Capacity>.Failure(CapacityErrors.ReleaseBelowZero);

            return Result<Capacity>.Success(new Capacity(MaxSlots, UsedSlots - slots));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return MaxSlots;
            yield return UsedSlots;
        }
    }
}
