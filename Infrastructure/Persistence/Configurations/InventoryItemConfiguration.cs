using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.HasKey(i => i.Id);

            // Configure StorageLocation Value Object
            builder.OwnsOne(i => i.Location, location =>
            {
                

                location.Property(l => l.Aisle)
                    .HasColumnName("Aisle")
                    .HasMaxLength(50);

                location.Property(l => l.Shelf)
                    .HasColumnName("Shelf")
                    .HasMaxLength(50);

                location.Property(l => l.Bin)
                    .HasColumnName("Bin")
                    .HasMaxLength(50);
            });
        }
    }
}