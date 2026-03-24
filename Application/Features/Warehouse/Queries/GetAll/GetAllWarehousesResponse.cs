using Domain.Warehouse.ValueObjects;

namespace Application.Warehouses.GetAllWarehouses
{
    // ──────────────────────────── Response ───────────────────────────

    public sealed record GetAllWarehousesResponse(
        WarehouseId Id,
        string Name,
        bool IsActive,
        string City,
        string Country,
        int TotalLocations
        //int TotalUsedSlots,
        //int TotalMaxSlots
        );
}