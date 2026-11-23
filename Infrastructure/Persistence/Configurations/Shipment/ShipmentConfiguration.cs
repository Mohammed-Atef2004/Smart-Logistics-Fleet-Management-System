using Domain.Shipments;
using Domain.Shipments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Shipments;

/// <summary>
/// EF Core mapping for the Shipment aggregate. Owned collections are part of
/// the aggregate, so they are mapped with <c>OwnsMany</c> and loaded with the
/// owner. Column lengths mirror the validation rules so the database cannot
/// accept data the domain rejects.
/// </summary>
public sealed class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");

        // ─── Key ─────────────────────────────────────────────────
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, value => ShipmentId.From(value))
            .ValueGeneratedNever();

        // Optimistic concurrency: Touch() increments this on every change.
        builder.Property(s => s.Version)
            .IsConcurrencyToken();

        // ─── Scalar properties ───────────────────────────────────
        builder.Property(s => s.SenderId)
            .HasMaxLength(Shipment.MaxSenderIdLength)
            .IsRequired();

        builder.Property(s => s.RecipientName).HasMaxLength(200);
        builder.Property(s => s.RecipientPhone).HasMaxLength(50);
        builder.Property(s => s.SpecialInstructions).HasMaxLength(1000);
        builder.Property(s => s.CancellationReason).HasMaxLength(500);
        builder.Property(s => s.Priority).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.DeliveredAt);
        builder.Property(s => s.CancelledAt);

        // ─── Delivery address (owned) ────────────────────────────
        builder.OwnsOne(s => s.DestinationAddress, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200).IsRequired();
            address.Property(a => a.City).HasMaxLength(100).IsRequired();
            address.Property(a => a.State).HasMaxLength(100).IsRequired();
            address.Property(a => a.ZipCode).HasMaxLength(20).IsRequired();
            address.Property(a => a.Country).HasMaxLength(100).IsRequired();
            address.Property(a => a.ApartmentUnit).HasMaxLength(50);
            address.Ignore(a => a.FullAddress);
        });

        // ─── Tracking (owned) ────────────────────────────────────
        builder.OwnsOne(s => s.Tracking, tracking =>
        {
            tracking.Property(t => t.TrackingNumber).HasMaxLength(100).IsRequired();
            tracking.Property(t => t.Status).IsRequired();
            tracking.Property(t => t.StatusDescription).HasMaxLength(500).IsRequired();
            tracking.Property(t => t.CarrierName).HasMaxLength(200);
            tracking.Property(t => t.EstimatedDeliveryDate);

            // A tracking number identifies exactly one shipment.
            tracking.HasIndex(t => t.TrackingNumber).IsUnique();

            // Used by the list endpoint's status filter.
            tracking.HasIndex(t => t.Status);
        });

        // ─── Packages (owned collection) ─────────────────────────
        builder.OwnsMany(s => s.Packages, package =>
        {
            package.ToTable("ShipmentPackages");

            package.WithOwner().HasForeignKey("ShipmentId");

            package.HasKey(p => p.Id);
            package.Property(p => p.Id)
                .HasConversion(id => id.Value, value => PackageId.From(value))
                .ValueGeneratedNever();

            package.Property(p => p.Description).HasMaxLength(500).IsRequired();
            package.Property(p => p.ContentCategory).HasMaxLength(100);
            package.Property(p => p.Currency).HasMaxLength(3).IsRequired();
            package.Property(p => p.DeclaredValue).HasPrecision(18, 4);

            package.OwnsOne(p => p.Weight, weight =>
            {
                weight.Property(w => w.Value).HasPrecision(18, 4);
            });

            package.OwnsOne(p => p.Dimensions, dimensions =>
            {
                dimensions.Property(d => d.Length).HasPrecision(18, 4);
                dimensions.Property(d => d.Width).HasPrecision(18, 4);
                dimensions.Property(d => d.Height).HasPrecision(18, 4);
            });
        });

        // ─── Route points (owned collection) ─────────────────────
        builder.OwnsMany(s => s.Route, route =>
        {
            route.ToTable("ShipmentRoutePoints");

            route.WithOwner().HasForeignKey("ShipmentId");

            // RoutePoint now carries its own stable id instead of relying on
            // an EF shadow key that could collide when several points were
            // added in the same unit of work.
            route.HasKey(r => r.Id);
            route.Property(r => r.Id).ValueGeneratedNever();

            route.Property(r => r.Location).HasMaxLength(200).IsRequired();
            route.Property(r => r.Description).HasMaxLength(500).IsRequired();
            route.Property(r => r.ArrivedAt).HasColumnType("datetime2").IsRequired();
            route.Property(r => r.Type).IsRequired();
        });

        // ─── Auditing / soft delete ──────────────────────────────
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt);
        builder.Property(s => s.CreatedBy).HasMaxLength(100);
        builder.Property(s => s.UpdatedBy).HasMaxLength(100);
        builder.Property(s => s.IsDeleted).HasDefaultValue(false);
        builder.Property(s => s.DeletedAtUtc);

        builder.HasQueryFilter(s => !s.IsDeleted);

        // Computed properties are not columns.
        builder.Ignore(s => s.Status);
        builder.Ignore(s => s.TotalPackages);
        builder.Ignore(s => s.TotalWeightKg);
        builder.Ignore(s => s.TotalDeclaredValue);
        builder.Ignore(s => s.IsDelivered);
        builder.Ignore(s => s.IsCancelled);
        builder.Ignore(s => s.IsEditable);
        builder.Ignore(s => s.CurrentLocation);
        builder.Ignore(s => s.DomainEvents);
    }
}
