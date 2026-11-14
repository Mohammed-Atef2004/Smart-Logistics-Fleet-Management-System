using Domain.Shifts;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Shift.Commands.CompleteShift
{
    public sealed class CompleteShiftCommandHandler : IRequestHandler<CompleteShiftCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShiftRepository _shiftRepository;
        public CompleteShiftCommandHandler(IUnitOfWork unitOfWork, IShiftRepository shiftRepository)
        {
            _unitOfWork = unitOfWork;
            _shiftRepository = shiftRepository;
        }
        public async Task<Result> Handle(CompleteShiftCommand request, CancellationToken cancellationToken)
        {
            var result = await _shiftRepository.EntityQuery.SingleOrDefaultAsync(x => x.Id == request.ShiftId);
            if (result==null) throw new Exception("the shift with this Id is not found");
            var res= result.Cancel();
            if (res.IsFailure) throw new Exception(res.Error.Message.ToString());
             _shiftRepository.Update(result);
            await _unitOfWork.CompleteAsync();
            return res;
        }
    }
}