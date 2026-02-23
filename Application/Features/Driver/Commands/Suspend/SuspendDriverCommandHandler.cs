using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.Susbend
{
    public class SuspendDriverCommandHandler : IRequestHandler<SuspendDriverCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SuspendDriverCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(SuspendDriverCommand request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Drivers.EntityQuery.SingleOrDefaultAsync(d => d.Id == request.Id);
            if (driver is null)
                throw new Exception("Driver Not Found");
            driver.Suspend(request.Reason);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
