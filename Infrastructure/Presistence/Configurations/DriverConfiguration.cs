using Domain.Drivers;
using Domain.Drivers.ValueObjects;
using Domain.Shifts.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("Drivers");

        // Primary Key
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => new DriverId(value));

        // FullName
        builder.Property(d => d.FullName)
            .IsRequired()
            .HasMaxLength(200);

        // Status Enum
        builder.Property(d => d.Status)
            .HasConversion<string>()
            .IsRequired();

        // Current Shift (Nullable VO)
        builder.Property(d => d.CurrentShiftId)
         .HasConversion(
          id => id == null ? (Guid?)null : id.Value,
          value => value == null ? null : new ShiftId(value.Value))
          .IsRequired(false);

        // Driver License Value Object
        builder.OwnsOne(d => d.License, license =>
        {
            license.Property(l => l.LicenseNumber)
                .HasColumnName("LicenseNumber")
                .IsRequired()
                .HasMaxLength(50);

            license.Property(l => l.ExpiryDate)
                .HasColumnName("LicenseExpiryDate")
                .IsRequired();

            license.Property(l => l.Category)
                .HasColumnName("Category")
                .HasConversion<string>()
                .IsRequired();
        });

        // Driver Rating Value Object
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

        // Ignore Domain Events
        builder.Ignore(d => d.DomainEvents);
    }
}