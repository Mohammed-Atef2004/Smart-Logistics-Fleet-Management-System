using Domain.Inventory;
using Domain.SharedKernel;
using MediatR;

namespace Application.Inventory.GetAllInventoryItems
{
    // ──────────────────────────── Handler ────────────────────────────

    public sealed class GetAllInventoryItemsQueryHandler
        : IRequestHandler<GetAllInventoryItemsQuery, Result<IReadOnlyList<GetAllInventoryItemsResponse>>>
    {
        private readonly IInventoryItemRepository _readRepository;

        public GetAllInventoryItemsQueryHandler(IInventoryItemRepository readRepository)
            => _readRepository = readRepository;

        public async Task<Result<IReadOnlyList<GetAllInventoryItemsResponse>>> Handle(
            GetAllInventoryItemsQuery query,
            CancellationToken cancellationToken)
        {
            var items = _readRepository.EntityQuery.Where(q=>q.IsActive==query.ActiveOnly);
            return Result<IReadOnlyList<GetAllInventoryItemsResponse>>.Success(
                    items.Select(item => new GetAllInventoryItemsResponse(
                        item.Id.Value,
                        item.ProductInfo.Sku,
                        item.ProductInfo.Name,
                        item.StockLevel.Quantity,
                        item.StockLevel.NeedsReorder,
                        item.StockLevel.IsOutOfStock,
                        item.IsActive
                    )).ToList()
                );
        }
    }
}