using Domain.Interfaces.Repositories;
using Domain.Inventory;
using Domain.Inventory.Errors;
using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;
using Domain.Warehouse;
using Domain.Warehouse.Errors;
using Domain.Warehouse.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Warehouses.UnassignItemFromLocation
{

    public sealed class UnassignItemFromLocationCommandHandler
        : IRequestHandler<UnassignItemFromLocationCommand, Result>
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnassignItemFromLocationCommandHandler(
            IWarehouseRepository warehouseRepository,
            IInventoryItemRepository inventoryItemRepository,
            IUnitOfWork unitOfWork)
        {
            _warehouseRepository = warehouseRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            UnassignItemFromLocationCommand command,
            CancellationToken cancellationToken)
        {
            var warehouseId = WarehouseId.From(command.WarehouseId);
            var locationId = StorageLocationId.From(command.StorageLocationId);
            var itemId = InventoryItemId.From(command.InventoryItemId);

            var warehouse = await _warehouseRepository.EntityQuery.FirstOrDefaultAsync(c => c.Id == warehouseId, cancellationToken);
            if (warehouse is null)
                return Result.Failure(WarehouseErrors.NotFound);

            var item = await _inventoryItemRepository.EntityQuery.FirstOrDefaultAsync(c => c.Id == itemId, cancellationToken);
            if (item is null)
                return Result.Failure(InventoryErrors.NotFound);

            var warehouseResult = warehouse.UnassignItemFromLocation(locationId, itemId);
            if (warehouseResult.IsFailure)
                return Result.Failure(warehouseResult.Error);

            var itemResult = item.UnassignFromLocation();
            if (itemResult.IsFailure)
                return Result.Failure(itemResult.Error);

            _warehouseRepository.Update(warehouse);
            _inventoryItemRepository.Update(item);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
    }
}