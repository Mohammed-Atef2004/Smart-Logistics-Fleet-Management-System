using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // سيب الـ Id والـ Name للـ Identity Defaults عشان متضربش
        // ركز بس على الحاجات الزيادة بتاعتك

        builder.Property(r => r.Description)
            .HasMaxLength(250);

        // لو عايز تأكد على اسم الجدول
        builder.ToTable("Roles", "Users");
    }
}