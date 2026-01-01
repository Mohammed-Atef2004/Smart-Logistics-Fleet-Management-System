using Domain.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories.Shipment
{
    public interface IPackageRepository:IGenericRepository<Package>
    {
        Task Update(Package package);
    }
}
