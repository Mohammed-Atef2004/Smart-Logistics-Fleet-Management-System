using Domain.Drivers;
using Domain.Vehicles;
using Domain.Vehicles.Events;

namespace Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
       
        IVehicleRepository Vehicles { get; }
        IDriverRepository Drivers { get; }

        IVehicleUniquenessChecker VehicleUniquenessChecker { get; }


        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}