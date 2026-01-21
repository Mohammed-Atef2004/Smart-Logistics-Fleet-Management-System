using Domain.Interfaces.Repositories;
using Domain.Shipment.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Interfaces
{
    public interface IPackageRepository:IGenericRepository<Package>
    {
        Task Update(Package package);
    }
}
