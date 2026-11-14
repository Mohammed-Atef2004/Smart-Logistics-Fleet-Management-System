using Domain.Drivers;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Driver.Commands.RecordRating
{
    public sealed class RecordDriverRatingCommandHandler : IRequestHandler<RecordDriverRatingCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDriverRepository _driverRepository;
        public RecordDriverRatingCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository)
        {
            _unitOfWork = unitOfWork;
            _driverRepository = driverRepository;
        }
        public async Task<Unit> Handle(RecordDriverRatingCommand request, CancellationToken cancellationToken)
        {
            var driver = await _driverRepository.EntityQuery.SingleOrDefaultAsync(d => d.Id == request.DriverId);
            if (driver is null)
                throw new Exception("Driver Not Found");
            driver.RecordTripRating(request.Rating);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}