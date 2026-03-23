using FluentValidation;

namespace Application.Inventory.AddStock
{
    // ──────────────────────────── Validator ──────────────────────────

    public sealed class AddStockCommandValidator : AbstractValidator<AddStockCommand>
    {
        public AddStockCommandValidator()
        {
            RuleFor(x => x.InventoryItemId)
                .NotEmpty().WithMessage("Inventory item id is required.");

            RuleFor(x => x.Units)
                .GreaterThan(0).WithMessage("Units must be greater than zero.");
        }
    }
}