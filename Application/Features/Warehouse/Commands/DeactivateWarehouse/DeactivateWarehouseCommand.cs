namespace Application.Warehouses.DeactivateWarehouse
{

    public sealed record DeactivateWarehouseCommand(Guid WarehouseId) : IRequest<Result>;
}