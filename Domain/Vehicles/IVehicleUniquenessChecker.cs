using Domain.Vehicles;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles
{
    
    public interface IVehicleUniquenessChecker
    {
        bool IsPlateUnique(VehiclePlateNumber plate);
    }
}
