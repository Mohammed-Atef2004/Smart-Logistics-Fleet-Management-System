using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using Domain.Warehouse;
using Domain.Warehouse.ValueObjects;
using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;
using Domain.Warehouse.Errors;

namespace Domain.Tests.Warehouses
{
    public class WarehouseTests
    {
        private static Domain.Warehouse.Warehouse CreateSut()
        {
            var result = Domain.Warehouse.Warehouse.Create(
                WarehouseId.New(),
                "Main Warehouse",
                Address.Create("Cairo", "Street 1", "Egypt","car-150").Value);

            return result.Value;
        }

        // ───────────────────────── Create ─────────────────────────

        [Fact]
        public void Create_Should_ReturnSuccess()
        {
            var result = Domain.Warehouse.Warehouse.Create(
                WarehouseId.New(),
                "WH-1",
                Address.Create("City", "Street", "Country", "car-150").Value);

            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("WH-1");
        }

        [Fact]
        public void Create_WithEmptyName_ShouldFail()
        {
            var result = Domain.Warehouse.Warehouse.Create(
                WarehouseId.New(),
                "   ",
                Address.Create("City", "Street", "Country", "car-150").Value);

            result.IsFailure.Should().BeTrue();
        }

        // ───────────────────────── Storage Location ─────────────────────────

        [Fact]
        public void AddStorageLocation_Should_AddSuccessfully()
        {
            var warehouse = CreateSut();

            var result = warehouse.AddStorageLocation(
                StorageLocationId.New(),
                "Loc-1",
                Capacity.Create(100).Value);

            result.IsSuccess.Should().BeTrue();
            warehouse.StorageLocations.Should().HaveCount(1);
        }

        [Fact]
        public void AddStorageLocation_Duplicate_ShouldFail()
        {
            var warehouse = CreateSut();
            var id = StorageLocationId.New();

            warehouse.AddStorageLocation(id, "Loc-1", Capacity.Create(100).Value);
            var result = warehouse.AddStorageLocation(id, "Loc-1", Capacity.Create(100).Value);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(StorageLocationErrors.LocationAlreadyExists);
        }

        // ───────────────────────── Assign Item ─────────────────────────

        [Fact]
        public void AssignItemToLocation_Should_Succeed()
        {
            var warehouse = CreateSut();
            var locId = StorageLocationId.New();

            warehouse.AddStorageLocation(locId, "Loc-1", Capacity.Create(100).Value);

            var result = warehouse.AssignItemToLocation(locId, InventoryItemId.New());

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void AssignItem_ToMissingLocation_ShouldFail()
        {
            var warehouse = CreateSut();

            var result = warehouse.AssignItemToLocation(
                StorageLocationId.New(),
                InventoryItemId.New());

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(StorageLocationErrors.LocationNotFound);
        }

        // ───────────────────────── Unassign Item ─────────────────────────

        [Fact]
        public void UnassignItem_Should_Succeed()
        {
            var warehouse = CreateSut();
            var locId = StorageLocationId.New();
            var itemId = InventoryItemId.New();

            warehouse.AddStorageLocation(locId, "Loc-1", Capacity.Create(100).Value);
            warehouse.AssignItemToLocation(locId, itemId);

            var result = warehouse.UnassignItemFromLocation(locId, itemId);

            result.IsSuccess.Should().BeTrue();
        }

        // ───────────────────────── Remove Location ─────────────────────────

        [Fact]
        public void RemoveStorageLocation_Should_RemoveSuccessfully()
        {
            var warehouse = CreateSut();
            var locId = StorageLocationId.New();

            warehouse.AddStorageLocation(locId, "Loc-1", Capacity.Create(100).Value);

            var result = warehouse.RemoveStorageLocation(locId);

            result.IsSuccess.Should().BeTrue();
            warehouse.StorageLocations.Should().BeEmpty();
        }

        // ───────────────────────── Update Address ─────────────────────────

        [Fact]
        public void UpdateAddress_Should_UpdateSuccessfully()
        {
            var warehouse = CreateSut();

            var newAddress = Address.Create("Alex", "New Street", "Egypt", "car-150").Value;

            var result = warehouse.UpdateAddress(newAddress);

            result.IsSuccess.Should().BeTrue();
            warehouse.Address.Should().Be(newAddress);
        }

        // ───────────────────────── Deactivate ─────────────────────────

        [Fact]
        public void Deactivate_Should_Succeed_WhenNoItems()
        {
            var warehouse = CreateSut();

            var result = warehouse.Deactivate();

            result.IsSuccess.Should().BeTrue();
            warehouse.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Deactivate_WhenAlreadyInactive_ShouldFail()
        {
            var warehouse = CreateSut();
            warehouse.Deactivate();

            var result = warehouse.Deactivate();

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(WarehouseErrors.AlreadyInactive);
        }

        [Fact]
        public void Deactivate_WhenHasAssignedItems_ShouldFail()
        {
            var warehouse = CreateSut();
            var locId = StorageLocationId.New();

            warehouse.AddStorageLocation(locId, "Loc-1", Capacity.Create(100).Value);
            warehouse.AssignItemToLocation(locId, InventoryItemId.New());

            var result = warehouse.Deactivate();

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(WarehouseErrors.HasAssignedItems);
        }
    }
}
