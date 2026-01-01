using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories.Warehouse
{
    public interface IInventoryItemRepository:IGenericRepository<InventoryItem>
    {
        Task Update(InventoryItem inventoryItem);
    }
}
