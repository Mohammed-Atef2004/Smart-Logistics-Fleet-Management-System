using Application.Features.Shipment.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Mappings
{
    public class ShipmentProfile:Profile
    {
        public ShipmentProfile()
        {
           CreateMap<Domain.Shipments.Shipment, ShipmentListDto>().ReverseMap();
           CreateMap<Domain.Shipments.Shipment, ShipmentDetailsDto>().ReverseMap();
           CreateMap<Domain.Shipments.Package, PackageDto>().ReverseMap();
        }
    }
}
