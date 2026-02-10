using Domain.Vehicles;

namespace Domain.Vehicles
{
    /// <summary>
    /// Contract to check if a Vehicle PlateNumber is unique.
    /// Aggregate will call this to ensure invariants without knowing DB.
    /// </summary>
    public interface IVehicleUniquenessChecker
    {
        bool IsPlateUnique(VehiclePlateNumber plate);
    }
}
