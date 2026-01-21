using Domain.Notifications.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations.Notification
{
    public class NotificationConfiguration:IEntityTypeConfiguration<Domain.Notifications.Entities.Notification>
    {
        public void Configure(EntityTypeBuilder<Domain.Notifications.Entities.Notification> builder)
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Message)
                   .IsRequired()
                   .HasMaxLength(500);
            builder.Property(n => n.IsRead)
                   .IsRequired();
            
        }
    }
}
