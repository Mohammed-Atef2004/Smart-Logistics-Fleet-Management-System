using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Driver.Commands.Delete
{
    public class DeleteDriverCommandHandler:IRequestHandler<DeleteDriverCommand,Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDriverCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit>Handle(DeleteDriverCommand command,CancellationToken cancellationToken)
        {
            var result=await _unitOfWork.Drivers.GetByIdAsync(command.Id);
            _unitOfWork.Drivers.Delete(result);
           await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
