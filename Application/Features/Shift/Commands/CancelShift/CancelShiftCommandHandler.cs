using Microsoft.EntityFrameworkCore;

namespace Application.Features.Shift.Commands.CancelShift
{
    public sealed class CancelShiftCommandHandler : IRequestHandler<CancelShiftCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShiftRepository _shiftRepository;
        public CancelShiftCommandHandler(IUnitOfWork unitOfWork, IShiftRepository shiftRepository)
        {
            _unitOfWork = unitOfWork;
            _shiftRepository = shiftRepository;
        }
        public async Task<Result> Handle(CancelShiftCommand request, CancellationToken cancellationToken)
        {
            var shift = await _shiftRepository.EntityQuery.SingleOrDefaultAsync(x=>x.Id==request.ShiftId);
            if (shift is null) return Result.Failure(new("Shift.NotFound", "Not found"));
           var result= shift.Cancel();
            if(result.IsFailure)
                return Result.Failure(result.Error);
             _shiftRepository.Update(shift);
            await _unitOfWork.CompleteAsync();
            return result;
        }
    }
}