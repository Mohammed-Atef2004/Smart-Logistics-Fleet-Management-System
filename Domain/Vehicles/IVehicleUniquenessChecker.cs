using Domain.Vehicles;

namespace Domain.Vehicles
{
    
    public interface IVehicleUniquenessChecker
    {
        bool IsPlateUnique(VehiclePlateNumber plate);
    }
}
