using Application.Features.Vehicle.Commands.ScheduleMaintenance;
using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using Domain.Vehicles.ValueObjects;
using MediatR;

namespace Application.Features.Vehicles.Commands.ScheduleMaintenance;

public sealed class ScheduleMaintenanceCommandHandler : IRequestHandler<ScheduleMaintenanceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ScheduleMaintenanceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ScheduleMaintenanceCommand request, CancellationToken cancellationToken)
    {
        // 1. تحميل الـ Aggregate من الـ Repository
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(new VehicleId(request.VehicleId), cancellationToken);

        if (vehicle is null)
            return Result.Failure(new Error("Vehicle.NotFound", "The specified vehicle was not found."));

        var descriptionResult = MaintenanceDescription.Create(request.Description);
        if (descriptionResult.IsFailure)
            return Result.Failure(descriptionResult.Error);

        var result = vehicle.ScheduleMaintenance(request.ScheduledDate, descriptionResult.Value);

        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}