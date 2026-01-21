using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Interfaces
{
    public interface IShipmentRepository:IGenericRepository<Entities.Shipment>
    {
        Task Update(Entities.Shipment shipment);
    }
}
