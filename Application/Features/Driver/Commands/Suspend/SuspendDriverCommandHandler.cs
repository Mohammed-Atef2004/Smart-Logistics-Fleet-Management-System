using Domain.Drivers;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Driver.Commands.Susbend
{
    public sealed class SuspendDriverCommandHandler : IRequestHandler<SuspendDriverCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDriverRepository _driverRepository;
        public SuspendDriverCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository)
        {
            _unitOfWork = unitOfWork;
            _driverRepository = driverRepository;
        }
        public async Task<Unit> Handle(SuspendDriverCommand request, CancellationToken cancellationToken)
        {
            var driver = await _driverRepository.EntityQuery.SingleOrDefaultAsync(d => d.Id == request.Id);
            if (driver is null)
                throw new Exception("Driver Not Found");
            driver.Suspend(request.Reason);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}