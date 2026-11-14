using Application.Common.Interfaces;
using Domain.Drivers;
using Domain.Shifts;
using Domain.Shipments;
using Domain.Vehicles;

namespace Application.Common.Interfaces;

/// <summary>
/// Unit of work abstraction owned by the Application layer and implemented
/// by Infrastructure on top of the EF Core DbContext.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IVehicleRepository Vehicles { get; }
    IDriverRepository Drivers { get; }
    IShiftRepository Shifts { get; }
    IShipmentRepository Shipments { get; }

    IVehicleUniquenessChecker VehicleUniquenessChecker { get; }

    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
