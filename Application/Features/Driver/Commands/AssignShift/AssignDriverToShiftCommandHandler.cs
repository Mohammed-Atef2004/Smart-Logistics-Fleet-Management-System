using Application.Common.Interfaces;
using Application.Features.Driver.DomainServices;
using MediatR;

namespace Application.Features.Driver.Commands.AssignShift;

public sealed class AssignDriverToShiftCommandHandler : IRequestHandler<AssignDriverToShiftCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDriverShiftService _driverShiftService;

    public AssignDriverToShiftCommandHandler(IUnitOfWork unitOfWork, IDriverShiftService driverShiftService)
    {
        _unitOfWork = unitOfWork;
        _driverShiftService = driverShiftService;
    }

    public async Task<Unit> Handle(AssignDriverToShiftCommand request, CancellationToken cancellationToken)
    {
        var result = await _driverShiftService.AssignDriverToShift(request.DriverId, request.Start, request.End);
        if (result.IsFailure) throw new Exception(result.Error.Message);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return Unit.Value;
    }
}
