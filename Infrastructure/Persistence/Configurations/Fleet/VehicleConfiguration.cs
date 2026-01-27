using Domain.Fleet.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Fleet
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles", "Fleet");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.LicensePlate)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.HasIndex(v => v.LicensePlate)
                   .IsUnique();

            // Maintenance Records (One-to-Many)
            builder.HasMany(v => v.MaintenanceRecords)
                   .WithOne(m => m.Vehicle)
                   .HasForeignKey(m => m.VehicleId);

            //builder.Metadata.FindNavigation(nameof(Vehicle.MaintenanceRecords)).SetPropertyAccessMode(PropertyAccessMode.Field);

            // ✅ One-to-One (Dependent side + FK هنا)
            builder.HasOne(v => v.CurrentDriver)
                   .WithOne(d => d.CurrentVehicle)
                   .HasForeignKey<Vehicle>(v => v.CurrentDriverId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Soft Delete
            builder.HasQueryFilter(v => !v.IsDeleted);
        }
    }
}
