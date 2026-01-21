using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Notifications.Entities
{
    public class Alert : BaseEntity
    {
        public Guid NotificationId { get; private set; }
        public DateTime SentAt { get; private set; }

        private Alert() { }

        public Alert(Guid notificationId)
        {
            NotificationId = notificationId;
            SentAt = DateTime.UtcNow;
        }
    }
}
