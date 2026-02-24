using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shift.Commands.CancelShift
{
    public class CancelShiftCommandHandler : IRequestHandler<CancelShiftCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CancelShiftCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(CancelShiftCommand request, CancellationToken cancellationToken)
        {
            var shift = await _unitOfWork.Shifts.EntityQuery.SingleOrDefaultAsync(x=>x.Id==request.ShiftId);
            if (shift is null) return Result.Failure(new("Shift.NotFound", "Not found"));
           var result= shift.Cancel();
            if(result.IsFailure)
                return Result.Failure(result.Error);
             _unitOfWork.Shifts.Update(shift);
            await _unitOfWork.CompleteAsync();
            return result;
        }
    }
}
