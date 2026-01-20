using Domain.Fleet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Fleet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Users;

namespace Infrastructure.Persistence.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {

            builder.HasKey(d => d.Id);

           
            builder.Property(d => d.FullName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(d => d.LicenseNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(d => d.CurrentVehicle)
                .WithOne()
                .HasForeignKey<Driver>(d => d.CurrentVehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Metadata.FindNavigation(nameof(Driver.DomainEvents))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }
}