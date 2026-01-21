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
    public class TrackingUpdateConfiguration:IEntityTypeConfiguration<TrackingUpdate>
    {
        public void Configure(EntityTypeBuilder<TrackingUpdate> builder)
        {
            builder.HasKey(tu => tu.Id);
            builder.Property(tu => tu.Location)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(tu => tu.Timestamp)
                .IsRequired();
        }
    }
}
