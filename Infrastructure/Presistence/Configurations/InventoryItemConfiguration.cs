using Domain.Inventory.ValueObjects;
using Domain.InventoryItems;
using Domain.Shipments.ValueObjects;
using Domain.Warehouse.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItems");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .HasConversion(
                    id => id.Value,                 // InventoryItemId.Value (Guid)
                    value => InventoryItemId.From(value))
                .IsRequired();

            builder.Property(i => i.IsActive)
                .IsRequired();

            // Cross-aggregate refs — stored as nullable Guids
            builder.Property(i => i.WarehouseId)
                .HasConversion(
                    id => id == null ? (Guid?)null : id.Id,   // WarehouseId.Id (Guid)
                    value => value == null ? null : WarehouseId.From(value.Value))
                .HasColumnName("WarehouseId")
                .IsRequired(false);

            builder.Property(i => i.StorageLocationId)
                .HasConversion(
                    id => id == null ? (Guid?)null : id.Value, // StorageLocationId.Value (Guid)
                    value => value == null ? null : StorageLocationId.From(value.Value))
                .HasColumnName("StorageLocationId")
                .IsRequired(false);

            builder.OwnsOne(i => i.ProductInfo, productInfo =>
            {
                productInfo.Property(p => p.Sku)
                    .HasColumnName("Sku")
                    .HasMaxLength(100)
                    .IsRequired();

                productInfo.Property(p => p.Name)
                    .HasColumnName("ProductName")
                    .HasMaxLength(300)
                    .IsRequired();

                productInfo.Property(p => p.Description)
                    .HasColumnName("Description")
                    .HasMaxLength(1000)
                    .IsRequired(false);

                productInfo.HasIndex(p => p.Sku).IsUnique();
            });

            builder.OwnsOne(i => i.StockLevel, stockLevel =>
            {
                stockLevel.Property(s => s.Quantity)
                    .HasColumnName("Quantity")
                    .IsRequired();

                stockLevel.Property(s => s.ReorderThreshold)
                    .HasColumnName("ReorderThreshold")
                    .IsRequired();
            });

            // Weight is from Domain.Shipments.ValueObjects — owned, not a relationship
            builder.OwnsOne(i => i.Weight, weight =>
            {
                weight.Property(w => w.Value)
                    .HasColumnName("WeightValue")
                    .HasPrecision(18, 4)
                    .IsRequired();

                weight.Property(w => w.Unit)
                    .HasColumnName("WeightUnit")
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();
            });
        }
    }
}