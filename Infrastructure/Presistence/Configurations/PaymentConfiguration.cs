using Domain.Invoices.ValueObjects;
using Domain.Payments;
using Domain.Payments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Presistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            // PaymentId conversion
            builder.Property(p => p.Id)
                .HasConversion(
                    v => v.Value,
                    v => new PaymentId(v))
                .IsRequired();

            // InvoiceId conversion - reference للـ Invoice aggregate بس عن طريق ID
            builder.Property(p => p.InvoiceId)
                .HasConversion(
                    v => v.Id,
                    v => new InvoiceId(v))
                .IsRequired();

            builder.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            // PaymentMethod - OwnsOne لأنه Value Object
            builder.OwnsOne(p => p.Method, method =>
            {
                method.Property(m => m.Type)
                    .HasColumnName("PaymentMethodType")
                    .HasMaxLength(50)
                    .IsRequired();

                method.Property(m => m.Provider)
                    .HasColumnName("PaymentMethodProvider")
                    .HasMaxLength(100);

                method.Property(m => m.Last4Digits)
                    .HasColumnName("CardLast4Digits")
                    .HasMaxLength(4);
            });

            // TransactionInfo - OwnsOne لأنه Value Object - nullable لو لسه Pending
            builder.OwnsOne(p => p.Transaction, tx =>
            {
                tx.Property(t => t.TransactionReference)
                    .HasColumnName("TransactionReference")
                    .HasMaxLength(200);

                tx.Property(t => t.ProcessedAt)
                    .HasColumnName("ProcessedAt");

                tx.Property(t => t.FailureReason)
                    .HasColumnName("FailureReason")
                    .HasMaxLength(500);
            });

            // Indexes
            builder.HasIndex(p => p.InvoiceId);
            builder.HasIndex(p => p.Status);

            builder.ToTable("Payments");
        }
    }
}
