using Domain.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IRouteRepository:IGenericRepository<Route>
    {
        Task Update(Route route);
    }
}
