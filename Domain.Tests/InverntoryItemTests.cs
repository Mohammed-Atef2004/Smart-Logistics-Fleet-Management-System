using Xunit;
using FluentAssertions;
using System.Linq;
using Domain.InventoryItems;
using Domain.Inventory.ValueObjects;
using Domain.Shipments.ValueObjects;
using Domain.Warehouse.ValueObjects;
using Domain.Inventory.Errors;
using Domain.Inventory.Events;
namespace Domain.Tests;

public class InventoryItemTests
{
    private InventoryItem CreateItem(int quantity = 10, int reorderThreshold = 5)
    {
        return InventoryItem.Create(
            InventoryItemId.New(),
            ProductInfo.Create("SKU1", "Test Product").Value,
            StockLevel.Create(quantity, reorderThreshold).Value,
            Weight.Create(2,Domain.Shipments.Enums.WeightUnit.Kg).Value
        ).Value;
    }

    [Fact]
    public void AddStock_WhenActive_ShouldIncreaseQuantity_AndRaiseEvent()
    {
        var item = CreateItem();

        var result = item.AddStock(5);

        result.IsSuccess.Should().BeTrue();
        item.StockLevel.Quantity.Should().Be(15);
        item.DomainEvents.Should().Contain(e => e is StockAddedEvent);
    }

    [Fact]
    public void AddStock_WhenInactive_ShouldFail()
    {
        var item = CreateItem();
        item.Deactivate();

        var result = item.AddStock(5);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void RemoveStock_WhenEnoughQuantity_ShouldDecrease_AndRaiseEvent()
    {
        var item = CreateItem();

        var result = item.RemoveStock(5);

        result.IsSuccess.Should().BeTrue();
        item.StockLevel.Quantity.Should().Be(5);
        item.DomainEvents.Should().Contain(e => e is StockRemovedEvent);
    }

    [Fact]
    public void RemoveStock_WhenBelowThreshold_ShouldRaiseReorderEvent()
    {
        var item = CreateItem(quantity: 10, reorderThreshold: 8);

        item.RemoveStock(5);

        item.DomainEvents.Should().Contain(e => e is ReorderNeededEvent);
    }

    [Fact]
    public void RemoveStock_WhenZero_ShouldRaiseOutOfStockEvent()
    {
        var item = CreateItem(quantity: 5);

        item.RemoveStock(5);

        item.DomainEvents.Should().Contain(e => e is ItemOutOfStockEvent);
    }

    [Fact]
    public void RemoveStock_WhenInsufficient_ShouldFail()
    {
        var item = CreateItem();

        var result = item.RemoveStock(50);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void AssignToLocation_FirstTime_ShouldSucceed_AndRaiseEvent()
    {
        var item = CreateItem();

        var result = item.AssignToLocation(WarehouseId.New(), StorageLocationId.New());

        result.IsSuccess.Should().BeTrue();
        item.WarehouseId.Should().NotBeNull();
        item.DomainEvents.Should().Contain(e => e is ItemLocationAssignedEvent);
    }

    [Fact]
    public void AssignToLocation_WhenAlreadyAssigned_ShouldFail()
    {
        var item = CreateItem();
        item.AssignToLocation(WarehouseId.New(), StorageLocationId.New());

        var result = item.AssignToLocation(WarehouseId.New(), StorageLocationId.New());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.AlreadyAssignedToLocation);
    }

    [Fact]
    public void UnassignFromLocation_ShouldClearLocation_AndRaiseEvent()
    {
        var item = CreateItem();
        item.AssignToLocation(WarehouseId.New(), StorageLocationId.New());

        var result = item.UnassignFromLocation();

        result.IsSuccess.Should().BeTrue();
        item.WarehouseId.Should().BeNull();
        item.DomainEvents.Should().Contain(e => e is ItemLocationUnassignedEvent);
    }

    [Fact]
    public void Deactivate_WhenAssigned_ShouldFail()
    {
        var item = CreateItem();
        item.AssignToLocation(WarehouseId.New(), StorageLocationId.New());

        var result = item.Deactivate();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.CannotDeactivateAssignedItem);
    }

    [Fact]
    public void Deactivate_WhenValid_ShouldDeactivate_AndRaiseEvent()
    {
        var item = CreateItem();

        var result = item.Deactivate();

        result.IsSuccess.Should().BeTrue();
        item.IsActive.Should().BeFalse();
        item.DomainEvents.Should().Contain(e => e is InventoryItemDeactivatedEvent);
    }

    [Fact]
    public void UpdateProductInfo_WhenActive_ShouldUpdate()
    {
        var item = CreateItem();
        var newInfo = ProductInfo.Create("SKU2", "Updated").Value;

        var result = item.UpdateProductInfo(newInfo);

        result.IsSuccess.Should().BeTrue();
        item.ProductInfo.Sku.Should().Be("SKU2");
    }

    [Fact]
    public void UpdateProductInfo_WhenInactive_ShouldFail()
    {
        var item = CreateItem();
        item.Deactivate();

        var newInfo = ProductInfo.Create("SKU2", "Updated").Value;
        var result = item.UpdateProductInfo(newInfo);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateWeight_ShouldAlwaysSucceed()
    {
        var item = CreateItem();

        var result = item.UpdateWeight(Weight.Create(10,Domain.Shipments.Enums.WeightUnit.Kg).Value);

        result.IsSuccess.Should().BeTrue();
        item.Weight.Value.Should().Be(10);
    }
}