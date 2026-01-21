// Application/Features/Fleet/Vehicles/Commands/CompleteMaintenance/CompleteMaintenanceCommandHandler.cs
using Application.Features.Fleet.Vehicle.Commands.CompleteMaintenance;
using Domain.Interfaces.Repositories;
using MediatR;

public class CompleteMaintenanceCommandHandler : IRequestHandler<CompleteMaintenanceCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public CompleteMaintenanceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CompleteMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.VehicleId);

        if (vehicle == null) throw new Exception("Vehicle not found");

        vehicle.CompleteMaintenance();

        _unitOfWork.Vehicles.Update(vehicle);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Unit.Value;
    }
}