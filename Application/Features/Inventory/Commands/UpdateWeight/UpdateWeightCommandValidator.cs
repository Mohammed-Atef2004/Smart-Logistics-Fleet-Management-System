using Domain.Shipments.Enums;
using FluentValidation;

namespace Application.Inventory.UpdateWeight
{
    // ──────────────────────────── Validator ──────────────────────────

    public sealed class UpdateWeightCommandValidator : AbstractValidator<UpdateWeightCommand>
    {
        public UpdateWeightCommandValidator()
        {
            RuleFor(x => x.InventoryItemId)
                .NotEmpty().WithMessage("Inventory item id is required.");

            RuleFor(x => x.WeightValue)
                .GreaterThan(0).WithMessage("Weight value must be greater than zero.");

            RuleFor(x => x.WeightUnit)
                .NotEmpty()
                .Must(u => Enum.TryParse<WeightUnit>(u, ignoreCase: true, out _))
                .WithMessage("Invalid weight unit. Valid values: Kg, Gram, Pound.");
        }
    }
}