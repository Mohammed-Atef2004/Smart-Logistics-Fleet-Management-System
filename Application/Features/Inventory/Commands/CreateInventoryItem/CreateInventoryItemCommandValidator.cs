using Domain.Shipments.Enums;
using FluentValidation;

namespace Application.Inventory.CreateInventoryItem
{
    // ──────────────────────────── Validator ──────────────────────────

    public sealed class CreateInventoryItemCommandValidator
        : AbstractValidator<CreateInventoryItemCommand>
    {
        public CreateInventoryItemCommandValidator()
        {
            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.");

            RuleFor(x => x.InitialQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Initial quantity cannot be negative.");

            RuleFor(x => x.ReorderThreshold)
                .GreaterThanOrEqualTo(0).WithMessage("Reorder threshold cannot be negative.");

            RuleFor(x => x.WeightValue)
                .GreaterThan(0).WithMessage("Weight value must be greater than zero.");

            RuleFor(x => x.WeightUnit)
                .NotEmpty()
                .Must(u => Enum.TryParse<WeightUnit>(u, ignoreCase: true, out _))
                .WithMessage("Invalid weight unit. Valid values: Kg, Gram, Pound.");
        }
    }
}