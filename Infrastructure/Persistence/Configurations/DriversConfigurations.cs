using Domain.Fleet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(d => d.LicenseNumber)
                .HasMaxLength(50)
                .IsRequired();

            // Configure One-to-One with ApplicationUser
            builder.HasOne(d => d.ApplicationUser)
                .WithOne(u => u.DriverProfile)
                .HasForeignKey<Driver>(d => d.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure One-to-One with Vehicle (optional)
            builder.HasOne(d => d.Vehicle)
                .WithOne()
                .HasForeignKey<Driver>("VehicleId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(d => d.LicenseNumber).IsUnique();
            builder.HasIndex(d => d.ApplicationUserId).IsUnique();

            // Query Filter للـ Soft Delete
            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }
}