using Domain.Fleet.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Fleet
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.ToTable("Drivers", "Fleet");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.FullName)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(d => d.LicenseNumber)
                   .HasMaxLength(50)
                   .IsRequired();

            // ✅ One-to-One (Principal side)
            builder.HasOne(d => d.CurrentVehicle)
                   .WithOne(v => v.CurrentDriver);

            // Domain Events
           // builder.Metadata.FindNavigation(nameof(Driver.DomainEvents))
               //    .SetPropertyAccessMode(PropertyAccessMode.Field);

            // Soft Delete
            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }
}
