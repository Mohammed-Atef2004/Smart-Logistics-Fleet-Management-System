using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Shipment;

namespace Domain.Interfaces.Repositories.Shipment
{
    public interface IShipmentRepository:IGenericRepository<Domain.Shipment.Shipment>
    {
        Task Update(Domain.Shipment.Shipment shipment);
    }
}
