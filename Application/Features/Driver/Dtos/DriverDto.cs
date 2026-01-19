using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Dtos
{
    public class DriverDto
    {
        public string FullName { get; set; }
        public string LicenseNumber { get; set; }
        public Guid? CurrentVehicleId { get; set; }
    }
}
