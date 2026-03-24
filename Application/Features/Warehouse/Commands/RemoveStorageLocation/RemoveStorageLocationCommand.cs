
using Domain.SharedKernel;
using MediatR;

namespace Application.Warehouses.RemoveStorageLocation
{

    public sealed record RemoveStorageLocationCommand(
        Guid WarehouseId,
        Guid StorageLocationId) : IRequest<Result>;
}