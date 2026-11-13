using Domain.Warehouse;
using Infrastructure.Presistence.Data;
using Infrastructure.Repositories.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class WarehouseRepository:GenericRepository<Warehouse>,IWarehouseRepository
    {
        public WarehouseRepository(AppDbContext appDbContext) : base(appDbContext) { }
    }
}
