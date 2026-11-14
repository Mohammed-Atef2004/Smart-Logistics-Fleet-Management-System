using Domain.Drivers;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Driver.Commands.Reactivate
{
    public sealed class ReactivateDriverCommandHandler:IRequestHandler<ReactivateDriverCommand,Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDriverRepository _driverRepository;
        public ReactivateDriverCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository)
        {
            _unitOfWork = unitOfWork;
            _driverRepository = driverRepository;
        }
        public async Task<Unit> Handle(ReactivateDriverCommand request, CancellationToken cancellationToken)
        {
            var driver = await _driverRepository.EntityQuery.SingleOrDefaultAsync(d => d.Id == request.Id);
            if (driver is null)
                throw new Exception("Driver Not Found");
            driver.Reactivate();
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}