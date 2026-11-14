namespace Application.Warehouses.AddStorageLocation
{

    public sealed record AddStorageLocationCommand(
        Guid WarehouseId,
        string Name,
        int MaxSlots) : IRequest<Result<AddStorageLocationResponse>>;
}