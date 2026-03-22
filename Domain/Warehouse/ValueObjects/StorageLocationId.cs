using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.ValueObjects
{
    public record StorageLocationId(Guid Value)
    {
        public static StorageLocationId From(Guid Value) { return new StorageLocationId(Value); }
    }
   
}
