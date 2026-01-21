using Domain.Notifications.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations.Notification
{
    public class AlertConfiguration:IEntityTypeConfiguration<Alert>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Alert> builder)
        {
            builder.HasKey(a => a.Id);
              
        }
    }
}
