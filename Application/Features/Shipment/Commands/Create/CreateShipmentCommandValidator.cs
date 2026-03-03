using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.Create
{
    public class CreateShipmentCommandValidator:AbstractValidator<CreateShipmentCommand>
    {
        public CreateShipmentCommandValidator()
        {
            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("Sender ID is required.")
                .MaximumLength(50).WithMessage("Sender ID cannot exceed 50 characters.");
            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required.")
                .MaximumLength(100).WithMessage("Street cannot exceed 100 characters.");
            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(50).WithMessage("City cannot exceed 50 characters.");
            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(50).WithMessage("State cannot exceed 50 characters.");
            RuleFor(x => x.ZipCode)
                .NotEmpty().WithMessage("Zip Code is required.")
                .MaximumLength(20).WithMessage("Zip Code cannot exceed 20 characters.");
            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(50).WithMessage("Country cannot exceed 50 characters.");
            RuleFor(x => x.TrackingNumber)
                .NotEmpty().WithMessage("Tracking Number is required.")
                .MaximumLength(100).WithMessage("Tracking Number cannot exceed 100 characters.");
            RuleFor(x => x.RecipientName)
                .MaximumLength(100).WithMessage("Recipient Name cannot exceed 100 characters.");
            RuleFor(x => x.RecipientPhone)
                .MaximumLength(20).WithMessage("Recipient Phone cannot exceed 20 characters.");
            RuleFor(x => x.SpecialInstructions)
                .MaximumLength(500).WithMessage("Special Instructions cannot exceed 500 characters.");
        }
    }
}
