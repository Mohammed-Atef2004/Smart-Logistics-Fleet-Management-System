using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shift.Commands.Create
{
    public class CreateShiftCommandValidator:AbstractValidator<CreateShiftCommand>
    {
        public CreateShiftCommandValidator()
        {
            RuleFor(x => x.driverId)
                .NotEmpty().WithMessage("DriverId is required.");
          
        }
    }
}
