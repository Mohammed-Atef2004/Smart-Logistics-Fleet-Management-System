using Domain.Fleet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Fleet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            // Table mapping and schema
            builder.ToTable("Drivers", "Fleet");

            builder.HasKey(d => d.Id);

            // One-to-One relationship with ApplicationUser (Identity Module)
            // Ensures a unique link between a system user and a fleet driver
            builder.Property(d => d.ApplicationUserId)
                .IsRequired();

            builder.HasIndex(d => d.ApplicationUserId)
                .IsUnique();

            builder.Property(d => d.FullName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(d => d.LicenseNumber)
                .HasMaxLength(50)
                .IsRequired();

            // Optional One-to-One relationship with Vehicle
            // A driver can be assigned to one vehicle at a time
            builder.HasOne(d => d.CurrentVehicle)
                .WithOne()
                .HasForeignKey<Driver>(d => d.CurrentVehicleId)
                .OnDelete(DeleteBehavior.SetNull); // Keep driver record if vehicle is deleted

            // Access mode for Domain Events backing field (Encapsulation)
            builder.Metadata.FindNavigation(nameof(Driver.DomainEvents))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // Global Query Filter for Soft Delete
            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }
}