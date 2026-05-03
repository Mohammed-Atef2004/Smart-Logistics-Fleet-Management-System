using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.ValueObjects
{
    public record WarehouseId(Guid Id)
    {
        public static WarehouseId Create(Guid Id) { return new WarehouseId(Id); }
        public static WarehouseId New() { return new WarehouseId(Guid.NewGuid()); }
        public static WarehouseId From(Guid Id) {return new WarehouseId(Id); }
    }

        
}
