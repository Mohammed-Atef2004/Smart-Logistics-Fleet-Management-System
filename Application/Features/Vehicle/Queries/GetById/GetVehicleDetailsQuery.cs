using Application.Features.Vehicle.DTOs;

namespace Application.Features.Vehicle.Queries.GetById
{
    public record GetVehicleDetailsQuery(Guid VehicleId) : IRequest<Domain.SharedKernel.Result<VehicleDetailsDto>>;
}