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
       
        void Update(Vehicle vehicle);
    }
}
