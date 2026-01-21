using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.MaintenanceRecord.Commands.Create
{
    public class CreateMaintenanceRecordCommandValidator:AbstractValidator<CreateMaintenanceRecordCommand>
    {
        public CreateMaintenanceRecordCommandValidator()
        {
            RuleFor(m => m.MaintenanceRecordDto.VehicleId).NotEmpty();
            RuleFor(m => m.MaintenanceRecordDto.Description).NotEmpty().MaximumLength(500);
            RuleFor(m => m.MaintenanceRecordDto.Cost).GreaterThanOrEqualTo(0);
        }
    }
}
