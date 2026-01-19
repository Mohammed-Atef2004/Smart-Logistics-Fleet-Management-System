using Domain.Fleet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories.Fleet
{
    public interface IVehicleRepository:IGenericRepository<Vehicle>
    {
        Task<Vehicle?> GetVehicleWithMaintenanceAsync(Guid id);
        Task<List<Vehicle>> GetAllVehiclesWithMaintenanceAsync();
        void Update(Vehicle vehicle);
    }
}
