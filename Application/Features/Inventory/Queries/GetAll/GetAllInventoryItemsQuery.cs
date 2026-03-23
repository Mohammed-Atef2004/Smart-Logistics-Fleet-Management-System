using Domain.SharedKernel;
using MediatR;

namespace Application.Inventory.GetAllInventoryItems
{
    // ──────────────────────────── Query ──────────────────────────────

    public sealed record GetAllInventoryItemsQuery(bool ActiveOnly = true)
        : IRequest<Result<IReadOnlyList<GetAllInventoryItemsResponse>>>;
}