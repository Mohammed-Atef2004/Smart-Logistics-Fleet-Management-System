using Application.Features.Shipment.DTOs;

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