namespace Application.Features.Driver.Commands.RecordRating;

public record RecordDriverRatingCommand(Guid DriverId, double Rating) : IRequest<Result>;
