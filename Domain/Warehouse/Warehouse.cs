using Domain.Common;
using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;
using Domain.Warehouse.Errors;
using Domain.Warehouse.Events;
using Domain.Warehouse.Rules;
using Domain.Warehouse.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Warehouse
{
    public sealed class Warehouse : AggregateRoot<WarehouseId>
    {
        public string Name { get; private set; }
        public Address Address { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<StorageLocation> _storageLocations = new();
        public IReadOnlyCollection<StorageLocation> StorageLocations => _storageLocations.AsReadOnly();

        private Warehouse() { }

        private Warehouse(WarehouseId id, string name, Address address) : base(id)
        {
            Name = name;
            Address = address;
            IsActive = true;
        }

        // ──────────────────────────── Factory ────────────────────────────

        public static Result<Warehouse> Create(WarehouseId id, string name, Address address)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Warehouse>.Failure(WarehouseErrors.EmptyName);

            var warehouse = new Warehouse(id, name, address);
            warehouse.AddDomainEvent(new WarehouseCreatedEvent(id, name));

            return Result<Warehouse>.Success(warehouse);
        }

        // ──────────────────────────── Behaviour ──────────────────────────

        public Result AddStorageLocation(StorageLocationId locationId, string name, Capacity capacity)
        {
            var ruleResult = CheckRule(new WarehouseMustBeActiveRule(IsActive));
            if (ruleResult.IsFailure) return ruleResult;

            if (_storageLocations.Any(l => l.Id == locationId))
                return Result.Failure(StorageLocationErrors.LocationAlreadyExists);

            var locationResult = StorageLocation.Create(locationId, name, capacity);
            if (locationResult.IsFailure)
                return Result.Failure(locationResult.Error);

            _storageLocations.Add(locationResult.Value);
            AddDomainEvent(new StorageLocationAddedEvent(Id, locationId, name));

            return Result.Success();
        }

        public Result AssignItemToLocation(StorageLocationId locationId, InventoryItemId itemId)
        {
            var ruleResult = CheckRule(new WarehouseMustBeActiveRule(IsActive));
            if (ruleResult.IsFailure) return ruleResult;

            var location = GetLocation(locationId);
            if (location is null)
                return Result.Failure(StorageLocationErrors.LocationNotFound);

            var assignResult = location.AssignItem(itemId);
            if (assignResult.IsFailure)
                return Result.Failure(assignResult.Error);

            AddDomainEvent(new ItemAssignedToLocationEvent(Id, locationId, itemId));
            return Result.Success();
        }

        public Result UnassignItemFromLocation(StorageLocationId locationId, InventoryItemId itemId)
        {
            var location = GetLocation(locationId);
            if (location is null)
                return Result.Failure(StorageLocationErrors.LocationNotFound);

            var unassignResult = location.UnassignItem(itemId);
            if (unassignResult.IsFailure)
                return Result.Failure(unassignResult.Error);

            AddDomainEvent(new ItemUnassignedFromLocationEvent(Id, locationId, itemId));
            return Result.Success();
        }

        public Result RemoveStorageLocation(StorageLocationId locationId)
        {
            var location = GetLocation(locationId);
            if (location is null)
                return Result.Failure(StorageLocationErrors.LocationNotFound);

            var deactivateResult = location.Deactivate();
            if (deactivateResult.IsFailure)
                return Result.Failure(deactivateResult.Error);

            _storageLocations.Remove(location);
            AddDomainEvent(new StorageLocationRemovedEvent(Id, locationId));

            return Result.Success();
        }

        public Result UpdateAddress(Address newAddress)
        {
            var ruleResult = CheckRule(new WarehouseMustBeActiveRule(IsActive));
            if (ruleResult.IsFailure) return ruleResult;

            Address = newAddress;
            AddDomainEvent(new WarehouseAddressUpdatedEvent(Id, newAddress));

            return Result.Success();
        }

        public Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure(WarehouseErrors.AlreadyInactive);

            if (_storageLocations.Any(l => l.AssignedItems.Any()))
                return Result.Failure(WarehouseErrors.HasAssignedItems);

            IsActive = false;
            AddDomainEvent(new WarehouseDeactivatedEvent(Id));

            return Result.Success();
        }

        // ──────────────────────────── Helpers ────────────────────────────

        private StorageLocation? GetLocation(StorageLocationId locationId)
            => _storageLocations.FirstOrDefault(l => l.Id == locationId);
    }
}

