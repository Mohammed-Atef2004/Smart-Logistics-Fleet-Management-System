using Domain.Vehicles;
using Domain.Vehicles.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Persistence.Configurations.Vehicles;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        // -------------------------
        // Key
        // -------------------------
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasConversion(
                id => id.Value,
                value => new VehicleId(value))
            .ValueGeneratedNever();

        // -------------------------
        // Plate number (uses HasConversion instead of OwnsOne)
        // -------------------------
        builder.Property(v => v.PlateNumber)
            .HasConversion(
                p => p.Value,
                v => VehiclePlateNumber.Create(v).Value) // Rebuilds the value object from the string
            .HasColumnName("PlateNumber")
            .HasMaxLength(20)
            .IsRequired();

        // -------------------------
        // Specification value object (kept as three columns)
        // -------------------------
        builder.OwnsOne(v => v.Specification, spec =>
        {
            spec.Property(s => s.Model)
                .HasColumnName("Model")
                .HasMaxLength(100)
                .IsRequired();

            spec.Property(s => s.Year)
                .HasColumnName("Year")
                .IsRequired();

            spec.Property(s => s.EngineType)
                .HasColumnName("EngineType")
                .HasMaxLength(50)
                .IsRequired();
        });

        // -------------------------
        // Fuel Consumption (Nullable VO)
        // -------------------------
        builder.OwnsOne(v => v.FuelConsumption, fuel =>
        {
            fuel.Property(f => f.Liters)
                .HasColumnName("FuelConsumption_LitersPer100Km");
        });

        // -------------------------
        // Enum Mapping
        // -------------------------
        builder.Property(v => v.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        // -------------------------
        // MaintenanceSchedules (Owned Entity Collection)
        // -------------------------
        builder.OwnsMany(v => v.MaintenanceSchedules, ms =>
        {
            ms.ToTable("VehicleMaintenanceSchedules");
            ms.WithOwner().HasForeignKey("VehicleId");
            ms.HasKey(m => m.Id);
            ms.Property(m => m.Id).ValueGeneratedNever();

            ms.OwnsOne(m => m.Description, d =>
            {
                d.Property(x => x.Value)
                    .HasColumnName("Description")
                    // Matches MaintenanceDescription.MaxLength (200) — the
                    // DB column previously allowed 500, wider than the
                    // domain invariant.
                    .HasMaxLength(MaintenanceDescription.MaxLength)
                    .IsRequired();
            });

            ms.Property(m => m.ScheduledDate).IsRequired();
            ms.Property(m => m.IsCompleted).IsRequired();
            ms.Property(m => m.CompletedAt);

            ms.OwnsOne(m => m.Remarks, r =>
            {
                r.Property(x => x.Value)
                    .HasColumnName("Remarks")
                    // Matches MaintenanceRemarks.MaxLength (500).
                    .HasMaxLength(MaintenanceRemarks.MaxLength);
            });
        });

        // -------------------------
        // Auditing Shadow Properties
        // -------------------------
        builder.Property<DateTime>("CreatedAt").IsRequired();
        builder.Property<DateTime?>("UpdatedAt");
        builder.Property<string>("CreatedBy").HasMaxLength(100);
        builder.Property<string>("UpdatedBy").HasMaxLength(100);
        builder.Property<bool>("IsDeleted").HasDefaultValue(false);
        builder.Property<DateTime?>("DeletedAtUtc");

        builder.HasQueryFilter(v => EF.Property<bool>(v, "IsDeleted") == false);
    }
}