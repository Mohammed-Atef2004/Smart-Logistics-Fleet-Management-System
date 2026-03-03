using Domain.Drivers;
using Domain.Shifts;
using Domain.Shipments;
using Domain.Vehicles;
using Domain.Vehicles.Events;
using System.Numerics;

namespace Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
       
        IVehicleRepository Vehicles { get; }
        IDriverRepository Drivers { get; }
        IShiftRepository Shifts { get; }
        IShipmentRepository Shipments { get; }

        IVehicleUniquenessChecker VehicleUniquenessChecker { get; }


        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}