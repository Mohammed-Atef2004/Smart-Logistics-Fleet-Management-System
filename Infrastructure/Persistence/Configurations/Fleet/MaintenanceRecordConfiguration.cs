using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Fleet.Entities;

namespace Infrastructure.Persistence.Configurations.Fleet
{
    public class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
    {
        public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
        {
            builder.ToTable("MaintenanceRecords", "Fleet");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Description)
                .HasMaxLength(500)
                .IsRequired();

            // Precision and Scale for financial data
            builder.Property(m => m.Cost)
                .HasColumnType("decimal(18,2)");

            builder.Property(m => m.MaintenanceDate)
                .IsRequired();
        }
    }
}