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

            // Repository واحد بس (اللي بينفذ الـ 2 interfaces)
            private IVehicleRepository? _vehicleRepository;

            public UnitOfWork(AppDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            // Repository (بينفذ كل من IVehicleRepository و IVehicleUniquenessChecker)
            public IVehicleRepository Vehicles =>
                _vehicleRepository ??= new VehicleRepository(_context);

            // نفس الـ Repository! (عشان الـ Domain)
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