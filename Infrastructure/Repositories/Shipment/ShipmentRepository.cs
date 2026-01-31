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
    public class ShipmentRepository:GenericRepository<Domain.Shipment.Entities.Shipment>, IShipmentRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ShipmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void Update(Domain.Shipment.Entities.Shipment shipment)
        {
            _context.Update(shipment);
        }

    }
}
