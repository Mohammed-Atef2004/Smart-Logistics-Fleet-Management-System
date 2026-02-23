using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.RecordRating
{
    public class RecordDriverRatingCommandHandler : IRequestHandler<RecordDriverRatingCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RecordDriverRatingCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(RecordDriverRatingCommand request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Drivers.EntityQuery.SingleOrDefaultAsync(d => d.Id == request.DriverId);
            if (driver is null)
                throw new Exception("Driver Not Found");
            driver.RecordTripRating(request.Rating);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
