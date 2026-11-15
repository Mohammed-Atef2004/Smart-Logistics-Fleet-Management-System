namespace Application.Features.Vehicle.Commands.RescheduleMaintenance;

public class RescheduleMaintenanceCommandValidator : AbstractValidator<RescheduleMaintenanceCommand>
{
    public RescheduleMaintenanceCommandValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();

        RuleFor(x => x.ScheduleId).NotEmpty();

        RuleFor(x => x.NewDate)
            .Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage("Maintenance date cannot be in the past.");
    }
}
