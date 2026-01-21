using Application.Features.Fleet.Vehicle.Commands.Create;
using Domain.Fleet;
using Domain.Fleet.Entities;
using Domain.Interfaces.Repositories;
using MediatR;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = new Vehicle(request.LicensePlate, request.Model, request.CurrentMileage);

        await _unitOfWork.Vehicles.AddAsync(vehicle);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return vehicle.Id;
    }
}