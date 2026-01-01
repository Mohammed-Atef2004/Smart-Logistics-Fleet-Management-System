using Domain.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories.Shipment
{
    public interface ITrackingUpdateRepository:IGenericRepository<TrackingUpdate>
    {
        Task Update(TrackingUpdate trackingUpdate);
    }
}
