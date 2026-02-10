using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Vehicles.Errors
{


    public static class VehicleErrors
    {
        public static Error NotAvailable =>
            new("Vehicle.NotAvailable", "Vehicle is not available");

        public static Error PlateAlreadyExists =>
            new("Vehicle.PlateAlreadyExists", "Plate number already exists");

        public static Error InvalidPlateNumber =>
            new("Vehicle.InvalidPlateNumber", "Invalid plate number");

        public static Error InvalidFuelConsumption =>
            new("Vehicle.InvalidFuelConsumption", "Invalid fuel consumption value");

        public static Error VehicleAlreadyRetired =>
            new("Vehicle.AlreadyRetired", "Vehicle is already retired");
    }

}
