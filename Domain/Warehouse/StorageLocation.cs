// StorageLocation.cs
using Domain.Common;
using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;
using Domain.Warehouse.Errors;
using Domain.Warehouse.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Warehouse
{
    public sealed class StorageLocation : Entity<StorageLocationId>
    {
        public string Name { get; private set; }
        public Capacity Capacity { get; private set; }
        public bool IsActive { get; private set; }

        // ⚡ AssignedItems as primitive Guid collection
        private readonly List<Guid> _assignedItemIds = new();
        public IReadOnlyCollection<Guid> AssignedItems => _assignedItemIds.AsReadOnly();

        private StorageLocation() { }

        private StorageLocation(StorageLocationId id, string name, Capacity capacity) : base(id)
        {
            Name = name;
            Capacity = capacity;
            IsActive = true;
        }

        internal static Result<StorageLocation> Create(StorageLocationId id, string name, Capacity capacity)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<StorageLocation>.Failure(StorageLocationErrors.LocationEmptyName);

            return Result<StorageLocation>.Success(new StorageLocation(id, name, capacity));
        }

        internal Result AssignItem(InventoryItemId itemId)
        {
            if (!IsActive)
                return Result.Failure(StorageLocationErrors.LocationInactive);

            if (_assignedItemIds.Contains(itemId.Value))
                return Result.Failure(StorageLocationErrors.ItemAlreadyAssigned);

            var reserveResult = Capacity.Reserve();
            if (reserveResult.IsFailure)
                return Result.Failure(reserveResult.Error);

            Capacity = reserveResult.Value;
            _assignedItemIds.Add(itemId.Value);

            return Result.Success();
        }

        internal Result UnassignItem(InventoryItemId itemId)
        {
            if (!_assignedItemIds.Contains(itemId.Value))
                return Result.Failure(StorageLocationErrors.ItemNotAssigned);

            var releaseResult = Capacity.Release();
            if (releaseResult.IsFailure)
                return Result.Failure(releaseResult.Error);

            Capacity = releaseResult.Value;
            _assignedItemIds.Remove(itemId.Value);

            return Result.Success();
        }

        internal Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure(StorageLocationErrors.LocationAlreadyInactive);

            if (_assignedItemIds.Any())
                return Result.Failure(StorageLocationErrors.LocationHasItems);

            IsActive = false;
            return Result.Success();
        }

        internal Result Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return Result.Failure(StorageLocationErrors.LocationEmptyName);

            Name = newName;
            return Result.Success();
        }
    }
}