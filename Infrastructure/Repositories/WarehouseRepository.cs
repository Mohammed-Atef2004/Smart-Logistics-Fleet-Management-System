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
        private readonly AppDbContext? _appDbContext;
        public WarehouseRepository(AppDbContext appDbContext) : base(appDbContext) { }
    }
}
