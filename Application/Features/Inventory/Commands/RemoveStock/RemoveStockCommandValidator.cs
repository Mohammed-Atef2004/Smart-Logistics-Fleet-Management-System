using FluentValidation;

namespace Application.Inventory.RemoveStock
{
    // ──────────────────────────── Validator ──────────────────────────

    public sealed class RemoveStockCommandValidator : AbstractValidator<RemoveStockCommand>
    {
        public RemoveStockCommandValidator()
        {
            RuleFor(x => x.InventoryItemId)
                .NotEmpty().WithMessage("Inventory item id is required.");

            RuleFor(x => x.Units)
                .GreaterThan(0).WithMessage("Units must be greater than zero.");
        }
    }
}