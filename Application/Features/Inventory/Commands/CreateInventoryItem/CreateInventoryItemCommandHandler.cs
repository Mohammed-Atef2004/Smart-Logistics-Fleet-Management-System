using Domain.Interfaces.Repositories;
using Domain.Inventory;
using Domain.Inventory.Errors;
using Domain.Inventory.ValueObjects;
using Domain.InventoryItems;
using Domain.SharedKernel;
using Domain.Shipments.Enums;
using Domain.Shipments.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Inventory.CreateInventoryItem
{
    // ──────────────────────────── Handler ────────────────────────────

    public sealed class CreateInventoryItemCommandHandler
        : IRequestHandler<CreateInventoryItemCommand, Result<CreateInventoryItemResponse>>
    {
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInventoryItemCommandHandler(
            IInventoryItemRepository inventoryItemRepository,
            IUnitOfWork unitOfWork)
        {
            _inventoryItemRepository = inventoryItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateInventoryItemResponse>> Handle(
            CreateInventoryItemCommand command,
            CancellationToken cancellationToken)
        {
            var skuExists = await _inventoryItemRepository.EntityQuery.FirstOrDefaultAsync(x=>x.ProductInfo.Sku==command.Sku, cancellationToken);
            if (skuExists!=null)
                return Result<CreateInventoryItemResponse>.Failure(ProductInfoErrors.DuplicateSku);

            var productInfoResult = ProductInfo.Create(command.Sku, command.Name, command.Description);
            if (productInfoResult.IsFailure)
                return Result<CreateInventoryItemResponse>.Failure(productInfoResult.Error);

            var stockLevelResult = StockLevel.Create(command.InitialQuantity, command.ReorderThreshold);
            if (stockLevelResult.IsFailure)
                return Result<CreateInventoryItemResponse>.Failure(stockLevelResult.Error);

            var unit = Enum.Parse<WeightUnit>(command.WeightUnit, ignoreCase: true);
            var weightResult = Weight.Create(command.WeightValue, unit);
            if (weightResult.IsFailure)
                return Result<CreateInventoryItemResponse>.Failure(weightResult.Error);

            var itemId = new InventoryItemId(Guid.NewGuid());

            var itemResult = InventoryItem.Create(
                itemId,
                productInfoResult.Value,
                stockLevelResult.Value,
                weightResult.Value);

            if (itemResult.IsFailure)
                return Result<CreateInventoryItemResponse>.Failure(itemResult.Error);

            await _inventoryItemRepository.AddAsync(itemResult.Value);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result<CreateInventoryItemResponse>.Success(
                new CreateInventoryItemResponse(itemId.Value));
        }
    }
}