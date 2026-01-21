using Domain.Shipment.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations.Shipment
{
    public class ShipmentConfiguration:IEntityTypeConfiguration<Domain.Shipment.Entities.Shipment>
    {
        public void Configure(EntityTypeBuilder<Domain.Shipment.Entities.Shipment> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.TrackingNumber)
                .IsRequired()
                .HasMaxLength(100);
         
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
