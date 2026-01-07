using Domain.Interfaces;
using Domain.Fleet; 
using MediatR;

namespace Application.Features.Vehicles.Commands.CreateVehicle;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        
        var vehicle = new Vehicle(
            request.LicensePlate,
            request.Model,
            request.Year 
        );

         await _unitOfWork.Vehicles.AddAsync(vehicle);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return vehicle.Id;
    }
}