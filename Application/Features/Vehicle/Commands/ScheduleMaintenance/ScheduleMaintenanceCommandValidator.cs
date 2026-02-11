using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.ScheduleMaintenance
{

    public class ScheduleMaintenanceCommandValidator : AbstractValidator<ScheduleMaintenanceCommand>
    {
        public ScheduleMaintenanceCommandValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty();

            RuleFor(x => x.ScheduledDate)
                .Must(date => date.Date >= DateTime.UtcNow.Date)
                .WithMessage("Maintenance date cannot be in the past.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(200);
        }
    }
}
