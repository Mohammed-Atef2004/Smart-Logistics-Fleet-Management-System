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

namespace Application.Warehouses.AssignItemToLocation
{
    // ──────────────────────────── Handler ────────────────────────────

    public sealed class AssignItemToLocationCommandHandler
        : IRequestHandler<AssignItemToLocationCommand, Result>
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignItemToLocationCommandHandler(
            IWarehouseRepository warehouseRepository,
            IInventoryItemRepository inventoryItemRepository,
            IUnitOfWork unitOfWork)
        {
            _warehouseRepository = warehouseRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            AssignItemToLocationCommand command,
            CancellationToken cancellationToken)
        {
            var warehouseId = WarehouseId.From(command.WarehouseId);
            var locationId = StorageLocationId.From(command.StorageLocationId);
            var itemId = InventoryItemId.From(command.InventoryItemId);

            var warehouse = await _warehouseRepository.EntityQuery.FirstOrDefaultAsync(c => c.Id == warehouseId, cancellationToken);
            if (warehouse is null)
                return Result.Failure(WarehouseErrors.NotFound);

            var item = await _inventoryItemRepository.EntityQuery.FirstOrDefaultAsync(c=>c.Id==itemId, cancellationToken);
            if (item is null)
                return Result.Failure(InventoryErrors.NotFound);

            var warehouseResult = warehouse.AssignItemToLocation(locationId, itemId);
            if (warehouseResult.IsFailure)
                return Result.Failure(warehouseResult.Error);

            var itemResult = item.AssignToLocation(warehouseId, locationId);
            if (itemResult.IsFailure)
                return Result.Failure(itemResult.Error);

            _warehouseRepository.Update(warehouse);
            _inventoryItemRepository.Update(item);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
    }
}