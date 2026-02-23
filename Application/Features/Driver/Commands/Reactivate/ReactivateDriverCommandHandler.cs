using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.Reactivate
{
    public class ReactivateDriverCommandHandler:IRequestHandler<ReactivateDriverCommand,Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReactivateDriverCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(ReactivateDriverCommand request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Drivers.EntityQuery.SingleOrDefaultAsync(d => d.Id == request.Id);
            if (driver is null)
                throw new Exception("Driver Not Found");
            driver.Reactivate();
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
