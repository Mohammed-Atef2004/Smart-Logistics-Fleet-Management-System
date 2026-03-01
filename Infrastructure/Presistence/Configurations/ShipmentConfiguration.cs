using Application.Features.Shipments.ValueObjects;
using Domain.Shipments;
using Domain.Shipments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Presistence.Configurations;

public sealed class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");

        // ───────────── Primary Key ─────────────
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => ShipmentId.From(value));

        builder.Property(s => s.Version)
            .IsConcurrencyToken();

        // ───────────── Scalar Props ─────────────
        builder.Property(s => s.SenderId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.RecipientName).HasMaxLength(200);
        builder.Property(s => s.RecipientPhone).HasMaxLength(50);
        builder.Property(s => s.SpecialInstructions).HasMaxLength(1000);
        builder.Property(s => s.CancellationReason).HasMaxLength(500);

        builder.Property(s => s.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.CreatedAt);
        builder.Property(s => s.UpdatedAt);
        builder.Property(s => s.DeliveredAt);
        builder.Property(s => s.CancelledAt);

        // ───────────── Delivery Address ─────────────
        builder.OwnsOne(s => s.DestinationAddress);

        // ───────────── Tracking ─────────────
        builder.OwnsOne(s => s.Tracking);

        // ───────────── Packages ─────────────
        builder.OwnsMany(s => s.Packages, pkg =>
        {
            pkg.ToTable("ShipmentPackages");

            pkg.WithOwner()
               .HasForeignKey("ShipmentId");

            pkg.HasKey(p => p.Id);

            pkg.Property(p => p.Id)
                .HasConversion(
                    id => id.Value,
                    value => PackageId.From(value));

            pkg.Property(p => p.Description)
                .HasMaxLength(500)
                .IsRequired();

            pkg.Property(p => p.DeclaredValue)
                .HasPrecision(18, 4);

            pkg.Property(p => p.Currency)
                .HasMaxLength(3);

            pkg.OwnsOne(p => p.Weight);
            pkg.OwnsOne(p => p.Dimensions);
        });

        // ───────────── Route Points (VO collection) ─────────────
        builder.OwnsMany(s => s.Route, rp =>
        {
            rp.ToTable("ShipmentRoutePoints");

            rp.WithOwner()
              .HasForeignKey("ShipmentId");

            rp.Property<Guid>("Id");
            rp.HasKey("Id");
        });

        builder.Ignore(s => s.DomainEvents);
    }
}