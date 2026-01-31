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
    public class PackageRepository : GenericRepository<Domain.Shipment.Entities.Package>, IPackageRepository
    {
        public PackageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }

        public void Update(Domain.Shipment.Entities.Package package)
        {
            _context.Update(package);
        }
    }
}
