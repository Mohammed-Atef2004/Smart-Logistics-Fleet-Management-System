using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Queries.GetById
{
    public class GetVehicleDetailsValidator : AbstractValidator<GetVehicleDetailsQuery>
    {
        public GetVehicleDetailsValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Vehicle ID is required.");
        }
    }
}
