using Domain.Shipment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ShipmentConfiguration:IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.TrackingNumber)
                .IsRequired()
                .HasMaxLength(100);
            builder.OwnsOne(s => s.Route, route =>
            {
                route.Property(r => r.From)
                    .IsRequired()
                    .HasMaxLength(200);
                route.Property(r => r.To)
                    .IsRequired()
                    .HasMaxLength(200);
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
