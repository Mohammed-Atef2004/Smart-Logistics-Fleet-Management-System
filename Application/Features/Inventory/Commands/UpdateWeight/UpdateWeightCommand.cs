namespace Application.Inventory.UpdateWeight
{

    public sealed record UpdateWeightCommand(
        Guid InventoryItemId,
        decimal WeightValue,
        string WeightUnit) : IRequest<Result>;
}