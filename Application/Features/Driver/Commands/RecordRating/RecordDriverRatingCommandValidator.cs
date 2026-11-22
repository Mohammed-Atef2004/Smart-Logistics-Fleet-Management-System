namespace Application.Features.Driver.Commands.RecordRating;

public sealed class RecordDriverRatingCommandValidator : AbstractValidator<RecordDriverRatingCommand>
{
    public RecordDriverRatingCommandValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}
