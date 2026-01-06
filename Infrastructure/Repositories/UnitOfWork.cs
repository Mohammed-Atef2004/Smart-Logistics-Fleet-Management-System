using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Repositories.Fleet;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Fleet;

namespace Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        // بنعرف الحقول كـ private عشان نشيل فيها النسخة لما تتعمل
        private IVehicleRepository _vehicles;
        private IDriverRepository _drivers;
        private IMaintenanceRecordRepository _maintenanceRecords;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IVehicleRepository Vehicles => _vehicles ??= new VehicleRepository(_context);

        public IDriverRepository Drivers => _drivers ??= new DriverRepository(_context);

        public IMaintenanceRecordRepository MaintenanceRecords => _maintenanceRecords ??= new MaintenanceRecordRepository(_context);

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
           
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}