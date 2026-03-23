
using Domain.SharedKernel;
using MediatR;

namespace Application.Inventory.CreateInventoryItem
{ 
    public sealed record CreateInventoryItemCommand(
        string Sku,
        string Name,
        string? Description,
        int InitialQuantity,
        int ReorderThreshold,
        decimal WeightValue,
        string WeightUnit) : IRequest<Result<CreateInventoryItemResponse>>;
}