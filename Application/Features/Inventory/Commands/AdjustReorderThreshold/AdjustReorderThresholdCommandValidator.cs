using FluentValidation;

namespace Application.Inventory.AdjustReorderThreshold
{
    // ──────────────────────────── Validator ──────────────────────────

    public sealed class AdjustReorderThresholdCommandValidator
        : AbstractValidator<AdjustReorderThresholdCommand>
    {
        public AdjustReorderThresholdCommandValidator()
        {
            RuleFor(x => x.InventoryItemId)
                .NotEmpty().WithMessage("Inventory item id is required.");

            RuleFor(x => x.NewThreshold)
                .GreaterThanOrEqualTo(0).WithMessage("Reorder threshold cannot be negative.");
        }
    }
}