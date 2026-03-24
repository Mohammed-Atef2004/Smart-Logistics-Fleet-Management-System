using FluentValidation;

namespace Application.Warehouses.AssignItemToLocation
{
    // ──────────────────────────── Validator ──────────────────────────

    public sealed class AssignItemToLocationCommandValidator
        : AbstractValidator<AssignItemToLocationCommand>
    {
        public AssignItemToLocationCommandValidator()
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