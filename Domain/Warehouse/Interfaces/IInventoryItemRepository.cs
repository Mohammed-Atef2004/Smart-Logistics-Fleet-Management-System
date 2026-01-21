using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.Interfaces
{
    public interface IInventoryItemRepository:IGenericRepository<InventoryItem>
    {
        Task Update(InventoryItem inventoryItem);
    }
}
