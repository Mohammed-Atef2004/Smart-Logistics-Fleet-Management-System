using Domain.Warehouse.ValueObjects;

namespace Application.Warehouses.GetWarehouseById
{

    public sealed record StorageLocationResponse(
        StorageLocationId Id,
        string Name,
        bool IsActive,
        int MaxSlots,
        int UsedSlots,
        int AvailableSlots,
        IReadOnlyList<Guid> AssignedItemIds);
}