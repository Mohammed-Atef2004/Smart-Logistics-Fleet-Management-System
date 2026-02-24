using Domain.Interfaces.Repositories;
using Domain.Shifts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.AssignShift
{
    public class AssignDriverToShiftCommandHandler : IRequestHandler<AssignDriverToShiftCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AssignDriverToShiftCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(AssignDriverToShiftCommand request, CancellationToken cancellationToken)
        {
            var service = new Domain.DomainServices.DriverShiftService(_unitOfWork.Drivers, _unitOfWork.Shifts);
            var result = await service.AssignDriverToShift(request.DriverId, request.Start, request.End);
            if (result.IsFailure) throw new Exception(result.Error.Message.ToString());
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
