using Domain.Interfaces.Repositories;
using Domain.InventoryItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Inventory
{
    public interface IInventoryItemRepository:IGenericRepository<InventoryItem>
    {
    }
}
