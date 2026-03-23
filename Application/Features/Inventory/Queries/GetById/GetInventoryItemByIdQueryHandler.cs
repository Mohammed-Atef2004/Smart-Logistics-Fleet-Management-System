using Domain.Inventory;
using Domain.Inventory.Errors;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Inventory.GetInventoryItemById
{
    // ──────────────────────────── Handler ────────────────────────────

    public sealed class GetInventoryItemByIdQueryHandler
        : IRequestHandler<GetInventoryItemByIdQuery, Result<GetInventoryItemByIdResponse>>
    {
        private readonly IInventoryItemRepository _readRepository;

        public GetInventoryItemByIdQueryHandler(IInventoryItemRepository readRepository)
            => _readRepository = readRepository;

        public async Task<Result<GetInventoryItemByIdResponse>> Handle(
            GetInventoryItemByIdQuery query,
            CancellationToken cancellationToken)
        {
            var item = await _readRepository.EntityQuery.FirstOrDefaultAsync(q=>q.Id==query.Id, cancellationToken);

            if (item is null)
                return Result<GetInventoryItemByIdResponse>.Failure(InventoryErrors.NotFound);

            return Result<GetInventoryItemByIdResponse>.Success(
             new GetInventoryItemByIdResponse(
                 item.Id.Value,
                 item.IsActive,
                 item.ProductInfo.Sku,
                 item.ProductInfo.Name,
                 item.ProductInfo.Description,
                 item.StockLevel.ReorderThreshold,
                 item.StockLevel.NeedsReorder,
                 item.StockLevel.IsOutOfStock,
                 item.Weight.Value,
                 item.Weight.Unit.ToString(),
                 item.WarehouseId,
                 item.StorageLocationId
             )
             );
        }
    }
}