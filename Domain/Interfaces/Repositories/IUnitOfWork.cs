using Domain.Fleet.Interfaces;

namespace Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IVehicleRepository Vehicles { get; }
        IDriverRepository Drivers { get; }
        IMaintenanceRecordRepository MaintenanceRecords { get; }


        // Renamed to CompleteAsync to follow your preferred naming
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}