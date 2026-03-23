using Domain.SharedKernel;
using MediatR;

namespace Application.Inventory.DeactivateInventoryItem
{

    public sealed record DeactivateInventoryItemCommand(Guid InventoryItemId) : IRequest<Result>;
}