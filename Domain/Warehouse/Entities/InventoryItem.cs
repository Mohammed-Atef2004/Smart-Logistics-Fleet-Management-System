using Domain.Common;
using Domain.Exceptions;
using Domain.Warehouse.Events;
using Domain.Warehouse.Rules;
using Domain.Warehouse.ValueObjects;

public class InventoryItem : AggregateRoot
{
    public string Name { get; private set; }
    public int Quantity { get; private set; }
    public StorageLocation Location { get; private set; }

    private InventoryItem() { }

    public InventoryItem(string name, int quantity, StorageLocation location)
    {
        CheckRule(new InventoryNameRequiredRule(name));
        CheckRule(new InventoryQuantityCannotBeNegativeRule(quantity));
        CheckRule(new StorageLocationRequiredRule(location));

        Name = name;
        Quantity = quantity;
        Location = location;
    }

    public void Move(StorageLocation newLocation)
    {
        CheckRule(new StorageLocationRequiredRule(newLocation));
        CheckRule(new DifferentLocationRule(Location, newLocation));

        var oldLocation = Location;
        Location = newLocation;

        AddDomainEvent(
            new InventoryMovedEvent(Id, oldLocation, newLocation)
        );
    }

    public void IncreaseQuantity(int amount)
    {
        CheckRule(new PositiveAmountRule(amount));

        Quantity += amount;
    }

    public void DecreaseQuantity(int amount)
    {
        CheckRule(new PositiveAmountRule(amount));
        CheckRule(new SufficientInventoryRule(Quantity, amount));

        Quantity -= amount;
    }
}
