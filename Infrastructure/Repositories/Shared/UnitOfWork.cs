using Domain.Driver;
using Domain.Interfaces.Repositories;
using Domain.Shipment;
using Domain.Vehicles.Events;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Fleet;
using Infrastructure.Repositories.Shipment;

namespace Infrastructure.Repositories.Shared
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        // بنعرف الحقول كـ private عشان نشيل فيها النسخة لما تتعمل
        private IVehicleRepository _vehicles;
        private IDriverRepository _drivers;
        private IMaintenanceRecordRepository _maintenanceRecords;
        private IShipmentRepository _shipments;
        private IPackageRepository _packageRecords;
        private ITrackingUpdateRepository _trackingUpdate;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public UnitOfWork(IVehicleRepository vehicles, IDriverRepository drivers, IMaintenanceRecordRepository maintenanceRecords, IShipmentRepository shipments, IPackageRepository packageRecords, ITrackingUpdateRepository trackingUpdate)
        {
            _vehicles = vehicles;
            _drivers = drivers;
            _maintenanceRecords = maintenanceRecords;
            _shipments = shipments;
            _packageRecords = packageRecords;
            _trackingUpdate = trackingUpdate;
        }

        public IVehicleRepository Vehicles => _vehicles ??= new VehicleRepository(_context);

        public IDriverRepository Drivers => _drivers ??= new DriverRepository(_context);

        public IMaintenanceRecordRepository MaintenanceRecords => _maintenanceRecords ??= new MaintenanceRecordRepository(_context);
        public IShipmentRepository ShipmentRecords =>_shipments??=new ShipmentRepository(_context);
        public IPackageRepository Packages=>_packageRecords??=new PackageRepository(_context);
        public ITrackingUpdateRepository TrackingUpdates=>_trackingUpdate??=new TrackingUpdateRepository(_context);
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