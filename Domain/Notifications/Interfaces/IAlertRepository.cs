using Domain.Interfaces.Repositories;
using Domain.Notifications.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Notifications.Interfaces
{
    public interface IAlertRepository:IGenericRepository<Alert>
    {
        Task Update(Alert alert);
    }
}
