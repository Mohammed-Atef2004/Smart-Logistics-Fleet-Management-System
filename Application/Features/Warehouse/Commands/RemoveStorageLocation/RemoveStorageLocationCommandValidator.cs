using FluentValidation;

namespace Application.Warehouses.RemoveStorageLocation
{
    // ──────────────────────────── Validator ──────────────────────────

    public sealed class RemoveStorageLocationCommandValidator
        : AbstractValidator<RemoveStorageLocationCommand>
    {
        public RemoveStorageLocationCommandValidator()
        {
            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage("Warehouse id is required.");

            RuleFor(x => x.StorageLocationId)
                .NotEmpty().WithMessage("Storage location id is required.");
        }
    }
}