using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shift.Commands.CompleteShift
{
    public class CompleteShiftCommandHandler : IRequestHandler<CompleteShiftCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompleteShiftCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(CompleteShiftCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Shifts.EntityQuery.SingleOrDefaultAsync(x => x.Id == request.ShiftId);
            if (result==null) throw new Exception("the shift with this Id is not found");
            var res= result.Cancel();
            if (res.IsFailure) throw new Exception(res.Error.Message.ToString());
             _unitOfWork.Shifts.Update(result);
            await _unitOfWork.CompleteAsync();
            return res;
        }
    }
}
