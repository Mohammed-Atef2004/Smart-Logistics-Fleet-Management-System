using Domain.Claims;
using Domain.Claims.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class InsuranceClaimConfiguration
    : IEntityTypeConfiguration<InsuranceClaim>
{
    public void Configure(EntityTypeBuilder<InsuranceClaim> builder)
    {
        builder.ToTable("InsuranceClaims");

        // =====================================================
        // Primary Key
        // =====================================================

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
        .HasConversion(
            id => id.Value,
            value => new ClaimId(value))
        .ValueGeneratedNever();

        // =====================================================
        // Claim Number
        // =====================================================

        builder.Property(c => c.ClaimNumber)
            .HasConversion(
                cn => cn.Value,
                value => ClaimNumber.From(value))
            .HasMaxLength(30)
            .IsRequired();

        // =====================================================
        // Scalar Properties
        // =====================================================

        builder.Property(c => c.ShipmentId)
            .IsRequired();

        builder.Property(c => c.CustomerId)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(c => c.RejectionReason)
            .HasMaxLength(500);

        builder.Property(c => c.SubmittedAt)
            .IsRequired();

        builder.Property(c => c.ReviewedAt);

        builder.Property(c => c.ProcessedAt);

        // =====================================================
        // ClaimAmount (Owned Value Object)
        // =====================================================

        builder.OwnsOne(c => c.ClaimAmount, money =>
        {
            money.Property(m => m.Value)
                .HasColumnName("ClaimAmount")
                .HasPrecision(18, 4)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("ClaimCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // =====================================================
        // ApprovedAmount (Nullable Owned VO)
        // =====================================================

        builder.OwnsOne(c => c.ApprovedAmount, money =>
        {
            money.Property(m => m.Value)
                .HasColumnName("ApprovedAmount")
                .HasPrecision(18, 4);

            money.Property(m => m.Currency)
                .HasColumnName("ApprovedCurrency")
                .HasMaxLength(3);
        });

        // =====================================================
        // SupportingDocument (Owned VO)
        // =====================================================

        builder.OwnsOne(c => c.SupportingDocument, doc =>
        {
            doc.Property(d => d.FileName)
                .HasColumnName("DocFileName")
                .HasMaxLength(255);

            doc.Property(d => d.FileUrl)
                .HasColumnName("DocFileUrl")
                .HasMaxLength(1000);

            doc.Property(d => d.ContentType)
                .HasColumnName("DocContentType")
                .HasMaxLength(100);

            doc.Property(d => d.FileSizeBytes)
                .HasColumnName("DocFileSizeBytes");

            doc.Property(d => d.UploadedAt)
                .HasColumnName("DocUploadedAt");
        });

        // =====================================================
        // ClaimItems (Owned Collection)
        // =====================================================

        builder.OwnsMany(c => c.Items, item =>
        {
            item.ToTable("ClaimItems");

            item.WithOwner()
                .HasForeignKey("ClaimId");

            item.HasKey(i => i.Id);

            item.Property(i => i.Description)
                .HasMaxLength(500)
                .IsRequired();

            item.Property(i => i.Quantity)
                .IsRequired();

            item.OwnsOne(i => i.UnitValue, money =>
            {
                money.Property(m => m.Value)
                    .HasColumnName("UnitAmount")
                    .HasPrecision(18, 4)
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("UnitCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        // =====================================================
        // Backing Field
        // =====================================================

        builder.Navigation(c => c.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(c => c.Items)
            .AutoInclude();

        // =====================================================
        // Indexes
        // =====================================================

        builder.HasIndex(c => c.ClaimNumber)
            .IsUnique();

        builder.HasIndex(c => c.Status);

        builder.HasIndex(c => c.ShipmentId);

        builder.HasIndex(c => c.CustomerId);

        builder.HasIndex(c => c.SubmittedAt);
    }
}