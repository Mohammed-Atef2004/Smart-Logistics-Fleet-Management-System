using FluentValidation;

namespace Application.Warehouses.GetWarehouseById
{
    // ──────────────────────────── Validator ──────────────────────────

    public sealed class GetWarehouseByIdQueryValidator
        : AbstractValidator<GetWarehouseByIdQuery>
    {
        public GetWarehouseByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Warehouse id is required.");
        }
    }
}