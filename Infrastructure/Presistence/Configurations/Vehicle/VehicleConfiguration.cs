using Domain.Vehicles;
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
        // Plate Number Value Object
        // -------------------------
        builder.OwnsOne(v => v.PlateNumber, plate =>
        {
            plate.Property(p => p.Value)
                .HasColumnName("PlateNumber")
                .HasMaxLength(20)
                .IsRequired();
        });

        // -------------------------
        // Specification Value Object (Record)
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
            fuel.Property(f => f.LitersPer100Km)
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

            // PK
            ms.HasKey(m => m.Id);

            ms.Property(m => m.Id)
                .ValueGeneratedNever();

            // -----------------------
            // Description Value Object
            // -----------------------
            ms.OwnsOne(m => m.Description, d =>
            {
                d.Property(x => x.Value)
                    .HasColumnName("Description")
                    .HasMaxLength(500)
                    .IsRequired();
            });

            // -----------------------
            // Scheduled Date
            // -----------------------
            ms.Property(m => m.ScheduledDate)
                .IsRequired();

            // -----------------------
            // Completion Flags
            // -----------------------
            ms.Property(m => m.IsCompleted)
                .IsRequired();

            ms.Property(m => m.CompletedAt);

            // -----------------------
            // Remarks Value Object
            // -----------------------
            ms.OwnsOne(m => m.Remarks, r =>
            {
                r.Property(x => x.Value)
                    .HasColumnName("Remarks")
                    .HasMaxLength(1000);
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
