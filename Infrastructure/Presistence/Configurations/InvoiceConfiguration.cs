using Domain.Invoices.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Presistence.Configurations
{
    public class InvoiceConfiguration:IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(i => i.Id);
            builder.ToTable("Invoices");
            builder.Property(i => i.Id)
                .HasConversion(
                    id => id.Id,
                    value => new InvoiceId(value));
            builder.Property(i => i.Status)
                .HasConversion<string>()
                .IsRequired();
            builder.OwnsMany(i => i.Items, item =>
            {
                item.WithOwner().HasForeignKey("InvoiceId");
                item.Property<int>("Id");
                item.HasKey("Id");
                item.Property(i => i.Description)
                    .IsRequired()
                    .HasMaxLength(500);
                item.Property(i => i.Price)
                    .IsRequired();
                item.Property(i => i.Quantity)
                    .IsRequired();
            });
        }
    }
}
