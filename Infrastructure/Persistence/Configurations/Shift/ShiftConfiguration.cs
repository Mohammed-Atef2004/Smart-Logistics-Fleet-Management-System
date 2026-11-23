using Domain.Drivers.ValueObjects;
using Domain.Shifts;
using Domain.Shifts.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Shifts;

public sealed class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("Shifts");

        // -------------------------
        // Key
        // -------------------------
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, value => new ShiftId(value))
            .ValueGeneratedNever();

        // -------------------------
        // Scalar properties
        // -------------------------
        builder.Property(s => s.DriverId)
            .HasConversion(id => id.Value, value => new DriverId(value))
            .IsRequired();

        builder.Property(s => s.ShiftStart)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(s => s.ShiftEnd)
            .HasColumnType("datetime2")
            .IsRequired();

        // Stored as text so the enum stays readable in the database.
        // Max length covers the longest member ("Cancelled" = 9).
        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.StartedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(s => s.EndedAtUtc)
            .HasColumnType("datetime2");

        // -------------------------
        // Indexes
        // -------------------------
        // Used by "does this driver already have a shift in this state" and
        // by the driver's shift history listing.
        builder.HasIndex(s => new { s.DriverId, s.Status });

        // Used by the overlap query (DriverId + range on ShiftStart/ShiftEnd).
        builder.HasIndex(s => new { s.DriverId, s.ShiftStart });

        // -------------------------
        // Auditing / soft delete
        // -------------------------
        builder.Property<DateTime>("CreatedAt").IsRequired();
        builder.Property<DateTime?>("UpdatedAt");
        builder.Property<string>("CreatedBy").HasMaxLength(100);
        builder.Property<string>("UpdatedBy").HasMaxLength(100);
        builder.Property<bool>("IsDeleted").HasDefaultValue(false);
        builder.Property<DateTime?>("DeletedAtUtc");

        builder.HasQueryFilter(s => EF.Property<bool>(s, "IsDeleted") == false);

        // Domain events are runtime-only; never persist them.
        builder.Ignore(s => s.DomainEvents);
    }
}
