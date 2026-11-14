using Domain.Warehouse.ValueObjects;

namespace Application.Warehouses.GetWarehouseById
{

    public sealed record GetWarehouseByIdQuery(WarehouseId Id)
        : IRequest<Result<GetWarehouseByIdResponse>>;
}