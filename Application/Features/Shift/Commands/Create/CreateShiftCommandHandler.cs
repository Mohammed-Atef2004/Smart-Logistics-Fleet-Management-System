using Domain.Shifts;
using Domain.Shifts.ValueObjects;
namespace Application.Features.Shift.Commands.Create
{
    public sealed class CreateShiftCommandHandler :IRequestHandler<CreateShiftCommand, Result<ShiftId>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShiftRepository _shiftRepository;
        public CreateShiftCommandHandler(IUnitOfWork unitOfWork, IShiftRepository shiftRepository)
        {
            _unitOfWork = unitOfWork;
            _shiftRepository = shiftRepository;
        }
        public async Task<Result<ShiftId>> Handle(CreateShiftCommand request, CancellationToken cancellationToken)
        {
            var shift = Domain.Shifts.Shift.Create(request.driverId, request.Start, request.End);
            if (shift.IsFailure) return Result<ShiftId>.Failure(shift.Error);
            await _shiftRepository.AddAsync(shift.Value);
            await _unitOfWork.CompleteAsync();
            return shift.IsSuccess ? Result<ShiftId>.Success(shift.Value.Id) : Result<ShiftId>.Failure(shift.Error);
        }
    }
}