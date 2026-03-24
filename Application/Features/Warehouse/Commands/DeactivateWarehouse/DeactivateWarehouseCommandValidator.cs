using FluentValidation;

namespace Application.Warehouses.DeactivateWarehouse
{
    // ──────────────────────────── Validator ──────────────────────────

    public sealed class DeactivateWarehouseCommandValidator
        : AbstractValidator<DeactivateWarehouseCommand>
    {
        public DeactivateWarehouseCommandValidator()
        {
            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage("Warehouse id is required.");
        }
    }
}