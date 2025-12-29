using Domain.Warehouse;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class InventoryItemConfiguration:IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.HasKey(ii => ii.Id);
            builder.Property(ii => ii.Name)
                .IsRequired()
                .HasMaxLength(150);
            builder.Property(ii => ii.Quantity)
                .IsRequired();
            builder.Property(ii => ii.Location)
                .HasMaxLength(100);
        }
    }
}
