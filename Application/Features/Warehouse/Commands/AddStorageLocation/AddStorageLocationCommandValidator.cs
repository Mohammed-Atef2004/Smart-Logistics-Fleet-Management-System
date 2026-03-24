using FluentValidation;

namespace Application.Warehouses.AddStorageLocation
{

    public sealed class AddStorageLocationCommandValidator : AbstractValidator<AddStorageLocationCommand>
    {
        public AddStorageLocationCommandValidator()
        {
            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage("Warehouse id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Storage location name is required.");

            RuleFor(x => x.MaxSlots)
                .GreaterThan(0).WithMessage("Max slots must be greater than zero.");
        }
    }
}