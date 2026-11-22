using Domain.Drivers;
using Domain.Drivers.Enums;
using Domain.Drivers.ValueObjects;
using Domain.Shifts.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Drivers;

public sealed class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("Drivers");

        // -------------------------
        // Key
        // -------------------------
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => new DriverId(value))
            .ValueGeneratedNever();

        // -------------------------
        // Scalar properties
        // -------------------------
        builder.Property(d => d.FullName)
            .IsRequired()
            // Matches Driver.MaxNameLength (100); the column used to allow
            // 200, wider than the domain invariant.
            .HasMaxLength(Driver.MaxNameLength);

        // Stored as text so the enum stays readable in the database.
        // Max length covers the longest member ("Terminated" = 10).
        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(d => d.Status);

        // -------------------------
        // Current shift (nullable VO, referenced by id only)
        // -------------------------
        builder.Property(d => d.CurrentShiftId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value == null ? null : new ShiftId(value.Value))
            .IsRequired(false);

        // -------------------------
        // License (owned value object)
        // -------------------------
        builder.OwnsOne(d => d.License, license =>
        {
            license.Property(l => l.LicenseNumber)
                .HasColumnName("LicenseNumber")
                .IsRequired()
                // Matches DriverLicense.MaxLicenseNumberLength (20).
                .HasMaxLength(DriverLicense.MaxLicenseNumberLength);

            license.Property(l => l.ExpiryDate)
                .HasColumnName("LicenseExpiryDate")
                .IsRequired();

            license.Property(l => l.Category)
                .HasColumnName("Category")
                .IsRequired()
                // Matches DriverLicense.MaxCategoryLength (10). The old
                // config had a no-op HasConversion<string>() on a string.
                .HasMaxLength(DriverLicense.MaxCategoryLength);
        });

        // -------------------------
        // Rating (owned value object)
        // -------------------------
        builder.OwnsOne(d => d.Rating, rating =>
        {
            rating.WithOwner();

            rating.Property(r => r.Value)
                .HasColumnName("RatingValue")
                .IsRequired();

            rating.Property(r => r.Count)
                .HasColumnName("RatingCount")
                .IsRequired();
        });

        // -------------------------
        // Auditing / soft delete
        // -------------------------
        builder.Property<DateTime>("CreatedAt").IsRequired();
        builder.Property<DateTime?>("UpdatedAt");
        builder.Property<string>("CreatedBy").HasMaxLength(100);
        builder.Property<string>("UpdatedBy").HasMaxLength(100);
        builder.Property<bool>("IsDeleted").HasDefaultValue(false);
        builder.Property<DateTime?>("DeletedAtUtc");

        builder.HasQueryFilter(d => EF.Property<bool>(d, "IsDeleted") == false);

        // Domain events are runtime-only; never persist them.
        builder.Ignore(d => d.DomainEvents);
    }
}
