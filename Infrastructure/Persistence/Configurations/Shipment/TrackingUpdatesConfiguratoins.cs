using Domain.Shipment.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Shipment
{
    public class TrackingUpdateConfiguration : IEntityTypeConfiguration<TrackingUpdate>
    {
        public void Configure(EntityTypeBuilder<TrackingUpdate> builder)
        {
            // 1. تحديد الجدول والـ Schema (اختياري حسب تنظيمك)
            builder.ToTable("TrackingUpdates", "Shipment");

            // 2. الـ Primary Key (موروث من BaseEntity)
            builder.HasKey(tu => tu.Id);

            // 3. الـ Properties
            builder.Property(tu => tu.Location)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(tu => tu.Notes)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(tu => tu.Timestamp)
                .IsRequired();

            // 4. العلاقة مع الـ Shipment (Foreign Key)
            // إحنا معرفين الـ ForeignKey كـ Guid صريح في الكلاس
            builder.Property(tu => tu.ShipmentId)
                .IsRequired();

            // ملحوظة: لو عايز تربطها بالـ Shipment Entity نفسه من هنا:
            // builder.HasOne<Domain.Shipment.Entities.Shipment>()
            //        .WithMany(s => s.TrackingUpdates)
            //        .HasForeignKey(tu => tu.ShipmentId)
            //        .OnDelete(DeleteBehavior.Cascade);

            // 5. Index للـ Performance عشان البحث بالـ ShipmentId سريع
            builder.HasIndex(tu => tu.ShipmentId);
        }
    }
}