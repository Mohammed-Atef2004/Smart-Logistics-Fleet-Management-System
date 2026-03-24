using FluentValidation;

namespace Application.Warehouses.UnassignItemFromLocation
{
    // ──────────────────────────── Validator ──────────────────────────

    public sealed class UnassignItemFromLocationCommandValidator
        : AbstractValidator<UnassignItemFromLocationCommand>
    {
        public UnassignItemFromLocationCommandValidator()
        {
            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage("Warehouse id is required.");

            RuleFor(x => x.StorageLocationId)
                .NotEmpty().WithMessage("Storage location id is required.");

            RuleFor(x => x.InventoryItemId)
                .NotEmpty().WithMessage("Inventory item id is required.");
        }
    }
}