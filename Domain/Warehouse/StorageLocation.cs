using Domain.Common;
using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;
using Domain.Warehouse.Errors;
using Domain.Warehouse.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse
{
    public sealed class StorageLocation : Entity<StorageLocationId>
    {
        public string Name { get; private set; }
        public Capacity Capacity { get; private set; }
        public bool IsActive { get; private set; }

        // Reference-only — no InventoryItem navigation property
        private readonly List<InventoryItemId> _assignedItems = new();
        public IReadOnlyCollection<InventoryItemId> AssignedItems => _assignedItems.AsReadOnly();

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

            if (_assignedItems.Contains(itemId))
                return Result.Failure(StorageLocationErrors.ItemAlreadyAssigned);

            var reserveResult = Capacity.Reserve();
            if (reserveResult.IsFailure)
                return Result.Failure(reserveResult.Error);

            Capacity = reserveResult.Value;
            _assignedItems.Add(itemId);

            return Result.Success();
        }

        internal Result UnassignItem(InventoryItemId itemId)
        {
            if (!_assignedItems.Contains(itemId))
                return Result.Failure(StorageLocationErrors.ItemNotAssigned);

            var releaseResult = Capacity.Release();
            if (releaseResult.IsFailure)
                return Result.Failure(releaseResult.Error);

            Capacity = releaseResult.Value;
            _assignedItems.Remove(itemId);

            return Result.Success();
        }

        internal Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure(StorageLocationErrors.LocationAlreadyInactive);

            if (_assignedItems.Any())
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

