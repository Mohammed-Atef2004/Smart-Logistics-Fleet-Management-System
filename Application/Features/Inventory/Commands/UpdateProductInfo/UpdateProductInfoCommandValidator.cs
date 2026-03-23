using FluentValidation;

namespace Application.Inventory.UpdateProductInfo
{
    // ──────────────────────────── Validator ──────────────────────────

    public sealed class UpdateProductInfoCommandValidator
        : AbstractValidator<UpdateProductInfoCommand>
    {
        public UpdateProductInfoCommandValidator()
        {
            RuleFor(x => x.InventoryItemId)
                .NotEmpty().WithMessage("Inventory item id is required.");

            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.");
        }
    }
}