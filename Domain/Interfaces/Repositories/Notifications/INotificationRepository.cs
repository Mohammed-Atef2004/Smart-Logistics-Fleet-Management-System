using Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories.Notifications
{
    public interface INotificationRepository:IGenericRepository<Notification>
    {
        Task Update(Notification notification);
    }
}
