using Microsoft.EntityFrameworkCore;

namespace Application.Features.Driver.Commands.UpdateName
{
    public sealed class UpdateDriverNameCommandHandler : IRequestHandler<UpdateDriverNameCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDriverRepository _driverRepository;
        public UpdateDriverNameCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository)
        {
            _unitOfWork = unitOfWork;
            _driverRepository = driverRepository;
        }
        public async Task<Unit> Handle(UpdateDriverNameCommand request, CancellationToken cancellationToken)
        {
            var driver = await _driverRepository.EntityQuery.SingleOrDefaultAsync(d => d.Id == request.Id);
            if (driver is null)
                throw new Exception("Driver Not Found");
            driver.UpdateName(request.FullName);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}