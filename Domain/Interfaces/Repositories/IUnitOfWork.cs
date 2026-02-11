using Domain.Vehicles;
using Domain.Vehicles.Events;

namespace Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Vehicle repository (also implements IVehicleUniquenessChecker)
        /// </summary>
        IVehicleRepository Vehicles { get; }

        /// <summary>
        /// Uniqueness checker (same instance as Vehicles repository)
        /// </summary>
        IVehicleUniquenessChecker VehicleUniquenessChecker { get; }

        /// <summary>
        /// Save all changes to the database
        /// </summary>
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}