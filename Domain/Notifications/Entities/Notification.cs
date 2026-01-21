using Domain.Common;
using Domain.Notifications.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Notifications.Entities
{
    public class Notification : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public string Message { get; private set; }
        public NotificationType Type { get; private set; }
        public bool IsRead { get; private set; }

        private Notification() { }

        public Notification(Guid userId, string message, NotificationType type)
        {
            UserId = userId;
            Message = message;
            Type = type;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
