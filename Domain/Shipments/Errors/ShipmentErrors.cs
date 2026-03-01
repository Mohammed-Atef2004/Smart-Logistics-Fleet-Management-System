using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Shipments.Errors
{
    public static class ShipmentErrors
    {
        public static Error EmptyCancelReason
            =>new("Shipment.EmptyCancelReason", "Cancel reason cannot be empty");
        public static Error EmptySenderId=>
            new("Shipment.EmptySenderId", "Sender ID cannot be empty");
        public static Error PackageNotFound = 
            new("Shipment.PackageNotFound", "The specified package was not found in the shipment");
    }
}
