using Domain.SharedKernel;
using Domain.Warehouse.ValueObjects;
using MediatR;

namespace Application.Warehouses.GetWarehouseById
{

    public sealed record GetWarehouseByIdQuery(WarehouseId Id)
        : IRequest<Result<GetWarehouseByIdResponse>>;
}