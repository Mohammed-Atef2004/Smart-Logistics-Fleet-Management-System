using FluentValidation;

namespace Application.Inventory.DeactivateInventoryItem
{

    public sealed class DeactivateInventoryItemCommandValidator
        : AbstractValidator<DeactivateInventoryItemCommand>
    {
        public DeactivateInventoryItemCommandValidator()
        {
            RuleFor(x => x.InventoryItemId)
                .NotEmpty().WithMessage("Inventory item id is required.");
        }
    }
}