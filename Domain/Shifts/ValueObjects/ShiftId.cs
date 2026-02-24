namespace Domain.Shifts.ValueObjects
{
    public sealed class ShiftId : IEquatable<ShiftId>
    {
        public Guid Value { get; }

        private ShiftId() { } // EF Core

        public ShiftId(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("ShiftId cannot be empty");

            Value = value;
        }

        public static ShiftId New() => new(Guid.NewGuid());

        public override bool Equals(object? obj)
            => obj is ShiftId other && Value.Equals(other.Value);

        public bool Equals(ShiftId? other)
            => other is not null && Value.Equals(other.Value);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }
}