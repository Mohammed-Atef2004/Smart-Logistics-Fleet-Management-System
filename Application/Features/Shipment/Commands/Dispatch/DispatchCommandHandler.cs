using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.Dispatch
{
    public class DispatchCommandHandler : IRequestHandler<DispatchCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DispatchCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(DispatchCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _unitOfWork.Shipments.EntityQuery.FirstOrDefaultAsync(x=>x.Id==request.Id);
            if (shipment is null)
                return Result.Failure(new Error("Shipment.NotFound", "Shipment not found."));
            shipment.Dispatch();
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
    }
}
