using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;
using MediatR;

namespace Application.Inventory.GetInventoryItemById
{
    // ──────────────────────────── Query ──────────────────────────────

    public sealed record GetInventoryItemByIdQuery(InventoryItemId Id)
        : IRequest<Result<GetInventoryItemByIdResponse>>;
}