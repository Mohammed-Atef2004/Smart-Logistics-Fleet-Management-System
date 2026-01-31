using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Mappings
{
    public class ShipmentProfile:Profile
    {
        public ShipmentProfile()
        {
            // Create your object-object mappings here
            CreateMap<Domain.Shipment.Entities.Shipment, DTOs.ShipmentDto>().ReverseMap();
        }
    }
}
