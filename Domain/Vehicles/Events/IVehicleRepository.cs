using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Vehicles.Events
{
    public interface IVehicleRepository:IGenericRepository<Vehicle>
    {
         Task<Vehicle?> GetVehicleWithDetailsAsync(Guid id);
    }
}
