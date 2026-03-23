using Domain.SharedKernel;
using MediatR;

namespace Application.Inventory.AdjustReorderThreshold
{

    public sealed record AdjustReorderThresholdCommand(
        Guid InventoryItemId,
        int NewThreshold) : IRequest<Result>;
}