using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Driver.Commands.Deactivate
{
    public class DeactivateDriverCommandHandler: IRequestHandler<DeactivateDriverCommand,Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeactivateDriverCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(DeactivateDriverCommand request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.Id);
            if (driver == null)
            {
                throw new Exception("Driver not found");
            }
            driver.Deactivate();
            _unitOfWork.Drivers.Update(driver);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
