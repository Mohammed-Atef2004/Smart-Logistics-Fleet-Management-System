using Domain.Driver;
using Domain.Shipment;
using Domain.Vehicles.Events;

namespace Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IVehicleRepository Vehicles { get; }
        IDriverRepository Drivers { get; }
        IMaintenanceRecordRepository MaintenanceRecords { get; }
        IShipmentRepository ShipmentRecords { get; }
        IPackageRepository Packages { get; }
        ITrackingUpdateRepository TrackingUpdates { get; }


        // Renamed to CompleteAsync to follow your preferred naming
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}