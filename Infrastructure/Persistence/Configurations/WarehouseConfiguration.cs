using Domain.Warehouse;
using Domain.Warehouse.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id)
                .HasConversion(
                    id => id.Id,                    // WarehouseId.Id (Guid)
                    value => WarehouseId.From(value))
                .IsRequired();

            builder.Property(w => w.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(w => w.IsActive)
                .IsRequired();

            builder.OwnsOne(w => w.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("Street")
                    .HasMaxLength(300)
                    .IsRequired();

                address.Property(a => a.City)
                    .HasColumnName("City")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.Country)
                    .HasColumnName("Country")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.ZipCode)
                    .HasColumnName("ZipCode")
                    .HasMaxLength(20)
                    .IsRequired();
            });

            builder.Ignore(w => w.StorageLocations);

            builder.HasMany<StorageLocation>("_storageLocations")
                .WithOne()
                .HasForeignKey("WarehouseId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation("_storageLocations")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}