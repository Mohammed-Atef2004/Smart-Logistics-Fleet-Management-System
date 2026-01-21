using Domain.Shipment.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Shipment
{
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.HasKey(r => r.Id);

            // Configure From Address
            builder.OwnsOne(r => r.From, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("FromStreet")
                    .HasMaxLength(200);

                address.Property(a => a.City)
                    .HasColumnName("FromCity")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.State)
                    .HasColumnName("FromState")
                    .HasMaxLength(100);

                address.Property(a => a.PostalCode)  // ✅ غيّرتها من ZipCode
                    .HasColumnName("FromPostalCode")
                    .HasMaxLength(20);

                address.Property(a => a.Country)
                    .HasColumnName("FromCountry")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            // Configure To Address
            builder.OwnsOne(r => r.To, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("ToStreet")
                    .HasMaxLength(200);

                address.Property(a => a.City)
                    .HasColumnName("ToCity")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.State)
                    .HasColumnName("ToState")
                    .HasMaxLength(100);

                address.Property(a => a.PostalCode)  // ✅ غيّرتها من ZipCode
                    .HasColumnName("ToPostalCode")
                    .HasMaxLength(20);

                address.Property(a => a.Country)
                    .HasColumnName("ToCountry")
                    .HasMaxLength(100)
                    .IsRequired();
            });
        }
    }
}