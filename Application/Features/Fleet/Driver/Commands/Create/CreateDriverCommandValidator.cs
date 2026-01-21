using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Driver.Commands.Create
{
    public class CreateDriverCommandValidator:AbstractValidator<CreateDriverCommand>
    {
        public CreateDriverCommandValidator()
        {
            RuleFor(d => d.driverDto.FullName).NotEmpty().MaximumLength(100);
            RuleFor(d => d.driverDto.LicenseNumber).NotEmpty().MaximumLength(50);
           
        }
    }
}
