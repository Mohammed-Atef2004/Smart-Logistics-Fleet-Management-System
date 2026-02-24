using Domain.Drivers.ValueObjects;
using Domain.Shifts;
using Domain.Shifts.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Presistence.Configurations
{
    public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
    {
        public void Configure(EntityTypeBuilder<Shift> b)
        {
            b.ToTable("Shifts");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .HasConversion(v => v.Value, v => new ShiftId(v));

            b.Property(x => x.DriverId)
                .HasConversion(v => v.Value, v => new DriverId(v));

            b.Property(x => x.Status).HasConversion<string>();

            b.HasIndex(x => new { x.DriverId, x.Status });
        }
    }
}
