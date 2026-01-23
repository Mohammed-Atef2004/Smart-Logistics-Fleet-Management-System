using Domain.Fleet.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles", "Fleet");
        builder.HasKey(v => v.Id);

        // Map the private field for the collection (Encapsulation)
        builder.Metadata.FindNavigation(nameof(Vehicle.MaintenanceRecords))
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(v => v.LicensePlate).HasMaxLength(20).IsRequired();
        builder.HasIndex(v => v.LicensePlate).IsUnique(); // لمنع تكرار اللوحات

        // Relationship One-to-Many
        builder.HasMany(v => v.MaintenanceRecords)
               .WithOne(m => m.Vehicle)
               .HasForeignKey(m => m.VehicleId);
        builder.HasOne(v => v.CurrentDriver)
                .WithMany()
                .HasForeignKey(v => v.CurrentDriverId)
                .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(v => !v.IsDeleted); // Soft Delete filter
    }
}