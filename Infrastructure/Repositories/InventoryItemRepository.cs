using Domain.Inventory;
using Domain.InventoryItems;
using Infrastructure.Presistence.Data;
using Infrastructure.Repositories.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class InventoryItemRepository:GenericRepository<InventoryItem>,IInventoryItemRepository
    {
        private readonly AppDbContext? _appDbContext;
        public InventoryItemRepository(AppDbContext appDbContext):base(appDbContext) { }
    }
}
