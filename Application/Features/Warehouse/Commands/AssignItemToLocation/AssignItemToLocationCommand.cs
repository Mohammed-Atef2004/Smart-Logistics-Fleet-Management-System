
using Domain.SharedKernel;
using MediatR;

namespace Application.Warehouses.AssignItemToLocation
{

    public sealed record AssignItemToLocationCommand(
        Guid WarehouseId,
        Guid StorageLocationId,
        Guid InventoryItemId) : IRequest<Result>;
}