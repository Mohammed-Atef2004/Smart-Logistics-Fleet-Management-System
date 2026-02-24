using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shift.Commands.StartShift
{
    public class StartShiftCommandHandler : IRequestHandler<StartShiftCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public StartShiftCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(StartShiftCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Shifts.EntityQuery.SingleOrDefaultAsync(x => x.Id == request.ShiftId);
            if (result == null) throw new Exception("there is no shift with this Id");
            var res=result.Start();
            if (res.IsFailure) return Result.Failure(res.Error);
            _unitOfWork.Shifts.Update(result);
            await _unitOfWork.CompleteAsync();
            return res;
        }
    }
}
