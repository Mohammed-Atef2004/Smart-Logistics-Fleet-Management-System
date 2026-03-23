
using Domain.SharedKernel;
using MediatR;

namespace Application.Inventory.AddStock
{
    public sealed record AddStockCommand(
        Guid InventoryItemId,
        int Units) : IRequest<Result>;
}