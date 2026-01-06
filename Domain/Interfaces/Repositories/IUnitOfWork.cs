using Domain.Interfaces.Repositories;
using Domain.Interfaces.Repositories.Fleet;

namespace Domain.Interfaces
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