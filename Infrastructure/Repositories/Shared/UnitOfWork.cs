using Domain.Drivers;
using Domain.Interfaces.Repositories;
using Domain.Vehicles;
using Infrastructure.Presistence.Data;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Vehicle;
using Infrastructure.Repositories.Vehicle.Infrastructure.Repositories;

namespace Infrastructure.Repositories.Shared
{
  

    namespace Infrastructure.Repositories.Shared
    {
        public sealed class UnitOfWork : IUnitOfWork
        {
            private readonly AppDbContext _context;

            private IVehicleRepository? _vehicleRepository;
            private IDriverRepository? _driverRepository;

            public UnitOfWork(AppDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public IVehicleRepository Vehicles =>
                _vehicleRepository ??= new VehicleRepository(_context);
            public IDriverRepository Drivers =>
                _driverRepository ??= new DriverRepository(_context);

            public IVehicleUniquenessChecker VehicleUniquenessChecker => Vehicles;

            public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }

            public void Dispose()
            {
                _context?.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}