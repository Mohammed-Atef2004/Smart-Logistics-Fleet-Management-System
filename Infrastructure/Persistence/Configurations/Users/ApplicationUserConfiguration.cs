using Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Users
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FullName)
                .HasMaxLength(200)
                .IsRequired();

            // Ignore Domain Events (هنحفظهم في جدول منفصل)
            //builder.Ignore(u => u.DomainEvents);

            // Configure relationship with Driver (already configured from Driver side)
        }
    }
}