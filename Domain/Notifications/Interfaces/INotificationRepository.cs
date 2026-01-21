using Domain.Interfaces.Repositories;
using Domain.Notifications.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Notifications.Interfaces
{
    public interface INotificationRepository:IGenericRepository<Notification>
    {
        Task Update(Notification notification);
    }
}
