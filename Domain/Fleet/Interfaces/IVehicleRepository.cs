using Domain.Fleet.Entities;
using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Fleet.Interfaces
{
    public interface IVehicleRepository:IGenericRepository<Vehicle>
    {
        Task<Vehicle?> GetVehicleWithMaintenanceAsync(Guid id);
        Task<List<Vehicle>> GetAllVehiclesWithMaintenanceAsync();
        void Update(Vehicle vehicle);
    }
}
