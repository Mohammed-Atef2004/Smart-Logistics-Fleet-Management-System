using Domain.Billing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Billing
{
    public class InsuranceClaimConfiguration : IEntityTypeConfiguration<InsuranceClaim>
    {
        public void Configure(EntityTypeBuilder<InsuranceClaim> builder)
        {
            builder.HasKey(ic => ic.Id);

            // Configure Money Value Object
            builder.OwnsOne(ic => ic.ClaimAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("ClaimAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("ClaimCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        }
    }
}