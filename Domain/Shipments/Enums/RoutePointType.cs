using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.Enums
{
    public enum RoutePointType
    {
        Origin = 0,
        Transit = 1,
        CustomsClearance = 2,
        SortingFacility = 3,
        OutForDelivery = 4,
        Destination = 5
    }
}
