using Domain.Shipment.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Shipment
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Domain.Shipment.Entities.Shipment>
    {
        public void Configure(EntityTypeBuilder<Domain.Shipment.Entities.Shipment> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.TrackingNumber)
                .IsRequired()
                .HasMaxLength(100);

            // ✅ Configure Route as Owned Type
            builder.OwnsOne(s => s.Route, route =>
            {
                // Origin Address
                route.OwnsOne(r => r.Origin, origin =>
                {
                    origin.Property(a => a.Street)
                        .HasColumnName("OriginStreet")
                        .HasMaxLength(200)
                        .IsRequired();

                    origin.Property(a => a.City)
                        .HasColumnName("OriginCity")
                        .HasMaxLength(100)
                        .IsRequired();

                    origin.Property(a => a.State)
                        .HasColumnName("OriginState")
                        .HasMaxLength(100);

                    origin.Property(a => a.Country)
                        .HasColumnName("OriginCountry")
                        .HasMaxLength(100)
                        .IsRequired();

                    origin.Property(a => a.PostalCode)
                        .HasColumnName("OriginPostalCode")
                        .HasMaxLength(20);
                });

                // Destination Address
                route.OwnsOne(r => r.Destination, destination =>
                {
                    destination.Property(a => a.Street)
                        .HasColumnName("DestinationStreet")
                        .HasMaxLength(200)
                        .IsRequired();

                    destination.Property(a => a.City)
                        .HasColumnName("DestinationCity")
                        .HasMaxLength(100)
                        .IsRequired();

                    destination.Property(a => a.State)
                        .HasColumnName("DestinationState")
                        .HasMaxLength(100);

                    destination.Property(a => a.Country)
                        .HasColumnName("DestinationCountry")
                        .HasMaxLength(100)
                        .IsRequired();

                    destination.Property(a => a.PostalCode)
                        .HasColumnName("DestinationPostalCode")
                        .HasMaxLength(20);
                });

                route.Property(r => r.EstimatedDistance)
                    .HasColumnName("EstimatedDistance")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            builder.HasMany(s => s.Packages)
                .WithOne()
                .HasForeignKey("ShipmentId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.TrackingUpdates)
                .WithOne()
                .HasForeignKey("ShipmentId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}