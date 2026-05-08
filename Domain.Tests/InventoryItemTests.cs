using System;
using Xunit;
using FluentAssertions;
using Domain.InventoryItems;
using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;
using Domain.Shipments.ValueObjects;
using Domain.Warehouse.ValueObjects;
using Domain.Inventory.Errors;
using Domain.Inventory.Enums;

namespace Domain.Tests.InventoryItems
{
    public class InventoryItemTests
    {
        private static InventoryItem CreateSut(int stock = 10)
        {
            var result = InventoryItem.Create(
                InventoryItemId.New(),
                ProductInfo.Create("SKU-1", "Test Product").Value,
                StockLevel.Create(stock, reorderThreshold: 5).Value,
                Weight.Create(2, Shipments.Enums.WeightUnit.Kg).Value);

            return result.Value;
        }

        // ───────────────────────── Create ─────────────────────────

        [Fact]
        public void Create_Should_ReturnSuccess_AndRaiseEvent()
        {
            var result = InventoryItem.Create(
                InventoryItemId.New(),
                ProductInfo.Create("SKU-1", "Test Product").Value,
                StockLevel.Create(10, 5).Value,
                Weight.Create(2,Shipments.Enums.WeightUnit.Gram).Value);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
        }

        // ───────────────────────── Add Stock ─────────────────────────

        [Fact]
        public void AddStock_Should_IncreaseQuantity()
        {
            var item = CreateSut(10);

            var result = item.AddStock(5);

            result.IsSuccess.Should().BeTrue();
            item.StockLevel.Quantity.Should().Be(15);
        }

        [Fact]
        public void AddStock_WhenInactive_ShouldFail()
        {
            var item = CreateSut();
            item.Deactivate();

            var result = item.AddStock(5);

            result.IsFailure.Should().BeTrue();
        }

        // ───────────────────────── Remove Stock ─────────────────────────

        [Fact]
        public void RemoveStock_Should_DecreaseQuantity()
        {
            var item = CreateSut(10);

            var result = item.RemoveStock(3);

            result.IsSuccess.Should().BeTrue();
            item.StockLevel.Quantity.Should().Be(7);
        }

        [Fact]
        public void RemoveStock_WhenOutOfStock_ShouldRaiseEvent()
        {
            var item = CreateSut(1);

            item.RemoveStock(1);

            item.StockLevel.IsOutOfStock.Should().BeTrue();
        }

        [Fact]
        public void RemoveStock_WhenNeedsReorder_ShouldRaiseEvent()
        {
            var item = CreateSut(6); // threshold 5

            item.RemoveStock(2);

            item.StockLevel.NeedsReorder.Should().BeTrue();
        }

        // ───────────────────────── Location ─────────────────────────

        [Fact]
        public void AssignToLocation_Should_AssignSuccessfully()
        {
            var item = CreateSut();

            var result = item.AssignToLocation(
                WarehouseId.New(),
                StorageLocationId.New());

            result.IsSuccess.Should().BeTrue();
            item.WarehouseId.Should().NotBeNull();
        }

        [Fact]
        public void AssignToLocation_WhenAlreadyAssigned_ShouldFail()
        {
            var item = CreateSut();

            item.AssignToLocation(WarehouseId.New(), StorageLocationId.New());

            var result = item.AssignToLocation(WarehouseId.New(), StorageLocationId.New());

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(InventoryErrors.AlreadyAssignedToLocation);
        }

        [Fact]
        public void UnassignFromLocation_Should_ClearLocation()
        {
            var item = CreateSut();

            item.AssignToLocation(WarehouseId.New(), StorageLocationId.New());
            var result = item.UnassignFromLocation();

            result.IsSuccess.Should().BeTrue();
            item.WarehouseId.Should().BeNull();
        }

        // ───────────────────────── Update Product Info ─────────────────────────

        [Fact]
        public void UpdateProductInfo_Should_UpdateSuccessfully()
        {
            var item = CreateSut();

            var newInfo = ProductInfo.Create("SKU-2", "New Product").Value;
            var result = item.UpdateProductInfo(newInfo);

            result.IsSuccess.Should().BeTrue();
            item.ProductInfo.Sku.Should().Be("SKU-2");
        }

        // ───────────────────────── Weight ─────────────────────────

        [Fact]
        public void UpdateWeight_Should_UpdateSuccessfully()
        {
            var item = CreateSut();

            var result = item.UpdateWeight(Weight.Create(5, Shipments.Enums.WeightUnit.Gram).Value);

            result.IsSuccess.Should().BeTrue();
            item.Weight.Value.Should().Be(5);
        }

        // ───────────────────────── Deactivate ─────────────────────────

        [Fact]
        public void Deactivate_Should_Work_WhenNotAssigned()
        {
            var item = CreateSut();

            var result = item.Deactivate();

            result.IsSuccess.Should().BeTrue();
            item.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Deactivate_WhenAlreadyInactive_ShouldFail()
        {
            var item = CreateSut();
            item.Deactivate();

            var result = item.Deactivate();

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(InventoryErrors.AlreadyInactive);
        }

        [Fact]
        public void Deactivate_WhenAssigned_ShouldFail()
        {
            var item = CreateSut();
            item.AssignToLocation(WarehouseId.New(), StorageLocationId.New());

            var result = item.Deactivate();

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(InventoryErrors.CannotDeactivateAssignedItem);
        }
    }
}
