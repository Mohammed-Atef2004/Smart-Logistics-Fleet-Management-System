using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.RecordRating
{
    public record RecordDriverRatingCommand(DriverId DriverId, double Rating) : IRequest<Unit>;
}