using Domain.SharedKernel;
using MediatR;

namespace Application.Inventory.RemoveStock
{

    public sealed record RemoveStockCommand(
        Guid InventoryItemId,
        int Units) : IRequest<Result>;
}