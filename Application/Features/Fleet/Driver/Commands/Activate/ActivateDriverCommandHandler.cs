using Application.Features.Fleet.Driver.Commands.Activate;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Driver.Commands.Activate
{
    public class ActivateDriverCommandHandler: IRequestHandler<ActivateDriverCommand,Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ActivateDriverCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(ActivateDriverCommand request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.Id);
            if (driver == null)
            {
                throw new Exception("Driver not found");
            }
            driver.Activate();
            _unitOfWork.Drivers.Update(driver);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
