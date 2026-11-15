namespace Application.Features.Vehicle.Commands.CompleteMaintenance;

public class CompleteMaintenanceCommandValidator : AbstractValidator<CompleteMaintenanceCommand>
{
    public CompleteMaintenanceCommandValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();

        RuleFor(x => x.ScheduleId).NotEmpty();

        RuleFor(x => x.Remarks)
            .MaximumLength(Domain.Vehicles.ValueObjects.MaintenanceRemarks.MaxLength)
            .When(x => x.Remarks is not null)
            .WithMessage($"Remarks cannot exceed {Domain.Vehicles.ValueObjects.MaintenanceRemarks.MaxLength} characters.");
    }
}
