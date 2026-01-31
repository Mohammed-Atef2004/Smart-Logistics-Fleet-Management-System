using Domain.Shipment.Interfaces;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Shipment
{
    public class TrackingUpdateRepository : GenericRepository<Domain.Shipment.Entities.TrackingUpdate>, ITrackingUpdateRepository
    {
        public TrackingUpdateRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }

        public void Update(Domain.Shipment.Entities.TrackingUpdate trackingUpdate)
        {
            _context.Update(trackingUpdate);
        }
    }
}
