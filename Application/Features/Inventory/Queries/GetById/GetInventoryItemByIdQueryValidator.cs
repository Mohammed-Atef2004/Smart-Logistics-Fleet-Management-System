using FluentValidation;

namespace Application.Inventory.GetInventoryItemById
{
    // ──────────────────────────── Validator ──────────────────────────

    public sealed class GetInventoryItemByIdQueryValidator
        : AbstractValidator<GetInventoryItemByIdQuery>
    {
        public GetInventoryItemByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Inventory item id is required.");
        }
    }
}