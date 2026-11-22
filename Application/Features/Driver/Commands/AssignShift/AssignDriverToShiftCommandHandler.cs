using Application.Features.Driver.DomainServices;
using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.AssignShift;

public sealed class AssignDriverToShiftCommandHandler : IRequestHandler<AssignDriverToShiftCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDriverShiftService _driverShiftService;

    public AssignDriverToShiftCommandHandler(IUnitOfWork unitOfWork, IDriverShiftService driverShiftService)
    {
        _unitOfWork = unitOfWork;
        _driverShiftService = driverShiftService;
    }

    public async Task<Result> Handle(AssignDriverToShiftCommand request, CancellationToken cancellationToken)
    {
        var result = await _driverShiftService.AssignDriverToShift(
            new DriverId(request.DriverId), request.Start, request.End);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
