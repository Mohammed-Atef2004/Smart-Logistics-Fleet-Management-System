using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.RegisterNewVehicle
{
    public sealed class RegisterNewVehicleCommandHandler
    : IRequestHandler<RegisterNewVehicleCommand, Result<VehicleId>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegisterNewVehicleCommandHandler> _logger;

        public RegisterNewVehicleCommandHandler(IUnitOfWork unitOfWork,
                                                ILogger<RegisterNewVehicleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<VehicleId>> Handle(RegisterNewVehicleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting vehicle registration for {Plate}", request.PlateNumber);
            var vplateNumber=  VehiclePlateNumber.Create(request.PlateNumber);
            var vehicleResult = Domain.Vehicles.Vehicle.Register(
                                vplateNumber.Value,
                                request.Specification,
                                _unitOfWork.VehicleUniquenessChecker
                            );

            if (vehicleResult.IsFailure)
            {
                _logger.LogWarning("Vehicle registration failed: {Error}", vehicleResult.Error);
                return Result<VehicleId>.Failure(vehicleResult.Error); 
            }

            // Add to DB
            await _unitOfWork.Vehicles.AddAsync(vehicleResult.Value);
            await _unitOfWork.CompleteAsync(cancellationToken);

            _logger.LogInformation("Vehicle registered with Id {VehicleId}", vehicleResult.Value.Id);
            return Result<VehicleId>.Success(vehicleResult.Value.Id); 

        }
    }
}
