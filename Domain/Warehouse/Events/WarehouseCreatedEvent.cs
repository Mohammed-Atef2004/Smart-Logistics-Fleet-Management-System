using Domain.Common;
using Domain.SharedKernel;
using Domain.Warehouse.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.Events
{
    public sealed record WarehouseCreatedEvent(
        WarehouseId WarehouseId,
        string Name) : DomainEvent;
}
