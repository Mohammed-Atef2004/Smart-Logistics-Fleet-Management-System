using Domain.Inventory.ValueObjects;

namespace Application.Inventory.GetInventoryItemById
{
    // ──────────────────────────── Query ──────────────────────────────

    public sealed record GetInventoryItemByIdQuery(InventoryItemId Id)
        : IRequest<Result<GetInventoryItemByIdResponse>>;
}