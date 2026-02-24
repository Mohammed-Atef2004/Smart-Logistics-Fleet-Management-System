using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Shifts;
using Domain.Shifts.ValueObjects;
using Domain.SharedKernel;
namespace Application.Features.Shift.Commands.Create
{
    public class CreateShiftCommandHandler :IRequestHandler<CreateShiftCommand, Result<ShiftId>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateShiftCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<ShiftId>> Handle(CreateShiftCommand request, CancellationToken cancellationToken)
        {
            var shift = Domain.Shifts.Shift.Create(request.driverId, request.Start, request.End);
            if (shift.IsFailure) return Result<ShiftId>.Failure(shift.Error);
            await _unitOfWork.Shifts.AddAsync(shift.Value);
            await _unitOfWork.CompleteAsync();
            return shift.IsSuccess ? Result<ShiftId>.Success(shift.Value.Id) : Result<ShiftId>.Failure(shift.Error);
        }
    }
}
