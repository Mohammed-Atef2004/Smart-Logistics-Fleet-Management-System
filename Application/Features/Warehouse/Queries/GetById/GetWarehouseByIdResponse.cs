using Domain.Warehouse.ValueObjects;

namespace Application.Warehouses.GetWarehouseById
{
    public sealed record GetWarehouseByIdResponse(
        WarehouseId Id,
        string Name,
        bool IsActive,
        string Street,
        string City,
        string Country,
        string ZipCode,
        IReadOnlyList<StorageLocationResponse> StorageLocations);
}