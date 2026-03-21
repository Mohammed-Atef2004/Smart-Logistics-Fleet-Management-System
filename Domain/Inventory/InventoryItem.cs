using Domain.Common;
using Domain.Inventory.Errors;
using Domain.Inventory.Events;
using Domain.Inventory.Rules;
using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;
using Domain.Shipments.ValueObjects;
using Domain.Warehouse.ValueObjects;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductInfo = Domain.Inventory.ValueObjects.ProductInfo;

namespace Domain.InventoryItems
{
    public sealed class InventoryItem : AggregateRoot<InventoryItemId>
    {
        public ProductInfo ProductInfo { get; private set; }
        public StockLevel StockLevel { get; private set; }
        public Weight Weight { get; private set; }
        public bool IsActive { get; private set; }

        public WarehouseId? WarehouseId { get; private set; }
        public StorageLocationId? StorageLocationId { get; private set; }

        private InventoryItem() { }

        private InventoryItem(
            InventoryItemId id,
            ProductInfo productInfo,
            StockLevel stockLevel,
            Domain.Shipments.ValueObjects.Weight weight) : base(id)
        {
            ProductInfo = productInfo;
            StockLevel = stockLevel;
            Weight = weight;
            IsActive = true;
        }

        // ──────────────────────────── Factory ────────────────────────────

        public static Result<InventoryItem> Create(
            InventoryItemId id,
            ProductInfo productInfo,
            StockLevel stockLevel,
            Weight weight)
        {
            var item = new InventoryItem(id, productInfo, stockLevel, weight);
            item.AddDomainEvent(new InventoryItemCreatedEvent(id, productInfo.Sku, stockLevel.Quantity));

            return Result<InventoryItem>.Success(item);
        }

        // ──────────────────────────── Stock Behaviour ─────────────────────

        public Result AddStock(int units)
        {
            var ruleResult = CheckRule(new InventoryItemMustBeActiveRule(IsActive));
            if (ruleResult.IsFailure) return ruleResult;

            var addResult = StockLevel.Add(units);
            if (addResult.IsFailure)
                return Result.Failure(addResult.Error);

            StockLevel = addResult.Value;
            AddDomainEvent(new StockAddedEvent(Id, units, StockLevel.Quantity));

            return Result.Success();
        }

        public Result RemoveStock(int units)
        {
            var ruleResult = CheckRule(new InventoryItemMustBeActiveRule(IsActive));
            if (ruleResult.IsFailure) return ruleResult;

            var removeResult = StockLevel.Remove(units);
            if (removeResult.IsFailure)
                return Result.Failure(removeResult.Error);

            StockLevel = removeResult.Value;
            AddDomainEvent(new StockRemovedEvent(Id, units, StockLevel.Quantity));

            if (StockLevel.NeedsReorder)
                AddDomainEvent(new ReorderNeededEvent(Id, ProductInfo.Sku, StockLevel.Quantity));

            if (StockLevel.IsOutOfStock)
                AddDomainEvent(new ItemOutOfStockEvent(Id, ProductInfo.Sku));

            return Result.Success();
        }

        public Result AdjustReorderThreshold(int newThreshold)
        {
            var adjustResult = StockLevel.AdjustReorderThreshold(newThreshold);
            if (adjustResult.IsFailure)
                return Result.Failure(adjustResult.Error);

            StockLevel = adjustResult.Value;
            return Result.Success();
        }

        // ──────────────────────────── Location Behaviour ──────────────────

        public Result AssignToLocation(WarehouseId warehouseId, StorageLocationId locationId)
        {
            var ruleResult = CheckRule(new InventoryItemMustBeActiveRule(IsActive));
            if (ruleResult.IsFailure) return ruleResult;

            if (WarehouseId is not null)
                return Result.Failure(InventoryErrors.AlreadyAssignedToLocation); 

            WarehouseId = warehouseId;
            StorageLocationId = locationId;

            AddDomainEvent(new ItemLocationAssignedEvent(Id, warehouseId, locationId));
            return Result.Success();
        }

        public Result UnassignFromLocation()
        {
            if (WarehouseId is null)
                return Result.Failure(InventoryErrors.NotAssignedToLocation); 

            var previousWarehouseId = WarehouseId;
            var previousLocationId = StorageLocationId;

            WarehouseId = null;
            StorageLocationId = null;

            AddDomainEvent(new ItemLocationUnassignedEvent(Id, previousWarehouseId!, previousLocationId!));
            return Result.Success();
        }

        // ──────────────────────────── Other Behaviour ─────────────────────

        public Result UpdateProductInfo(ProductInfo newProductInfo)
        {
            var ruleResult = CheckRule(new InventoryItemMustBeActiveRule(IsActive));
            if (ruleResult.IsFailure) return ruleResult;

            ProductInfo = newProductInfo;
            return Result.Success();
        }

        public Result UpdateWeight(Weight newWeight)
        {
            Weight = newWeight;
            return Result.Success();
        }

        public Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure(InventoryErrors.AlreadyInactive); 

            if (WarehouseId is not null)
                return Result.Failure(InventoryErrors.CannotDeactivateAssignedItem); 

            IsActive = false;
            AddDomainEvent(new InventoryItemDeactivatedEvent(Id));

            return Result.Success();
        }
    }
}