using FluentValidation;

namespace Application.Warehouses.UpdateAddress
{

    public sealed class UpdateWarehouseAddressCommandValidator
        : AbstractValidator<UpdateWarehouseAddressCommand>
    {
        public UpdateWarehouseAddressCommandValidator()
        {
            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage("Warehouse id is required.");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.");

            RuleFor(x => x.ZipCode)
                .NotEmpty().WithMessage("Zip code is required.");
        }
    }
}