using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.HireDriver
{
    public class HireDriverCommandValidator:AbstractValidator<HireDriverCommand>
    {
        public HireDriverCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");
            RuleFor(x => x.LicenseNumber)
                .NotEmpty().WithMessage("License number is required.")
                .MaximumLength(20).WithMessage("License number must not exceed 20 characters.");
            RuleFor(x => x.ExpiryDate)
                .GreaterThan(DateTime.Now).WithMessage("Expiry date must be in the future.");
            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.")
                .MaximumLength(10).WithMessage("Category must not exceed 10 characters.");
        }
    }
}
