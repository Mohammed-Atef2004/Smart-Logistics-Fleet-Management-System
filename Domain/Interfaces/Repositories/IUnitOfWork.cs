using Domain.Vehicles.Events;

namespace Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        //IVehicleRepository Vehicles { get; }


        // Renamed to CompleteAsync to follow your preferred naming
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}