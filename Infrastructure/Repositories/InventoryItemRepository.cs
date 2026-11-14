using Application.Common.Interfaces;
using Domain.Inventory;
using Infrastructure.Persistence.Data;
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
        public InventoryItemRepository(AppDbContext appDbContext):base(appDbContext) { }
    }
}