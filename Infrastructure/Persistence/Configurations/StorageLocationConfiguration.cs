using Domain.Warehouse;
using Domain.Warehouse.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class StorageLocationConfiguration : IEntityTypeConfiguration<StorageLocation>
    {
        public void Configure(EntityTypeBuilder<StorageLocation> builder)
        {
            builder.ToTable("StorageLocations");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .HasConversion(
                    id => id.Value,
                    value => StorageLocationId.From(value))
                .IsRequired();

            builder.Property(l => l.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(l => l.IsActive)
                .IsRequired();

            // FK must be same type as Warehouse PK (WarehouseId) for EF relationship matching
            builder.Property<WarehouseId>("WarehouseId")
                .HasConversion(
                    id => id.Id,
                    value => WarehouseId.From(value))
                .IsRequired();

            builder.OwnsOne(l => l.Capacity, capacity =>
            {
                capacity.Property(c => c.MaxSlots)
                    .HasColumnName("MaxSlots")
                    .IsRequired();

                capacity.Property(c => c.UsedSlots)
                    .HasColumnName("UsedSlots")
                    .IsRequired();
            });

            builder.Ignore(l => l.AssignedItems);

            builder.Property<List<Guid>>("_assignedItemIds")
                .HasField("_assignedItemIds")
                .HasColumnName("AssignedItemIds")
                .HasConversion(
                    ids => JsonSerializer.Serialize(ids, (JsonSerializerOptions?)null),
                    json => JsonSerializer.Deserialize<List<Guid>>(json, (JsonSerializerOptions?)null) ?? new List<Guid>())
                .Metadata.SetValueComparer(
                    new ValueComparer<List<Guid>>(
                        (a, b) => a != null && b != null && a.SequenceEqual(b),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));
        }
    }
}