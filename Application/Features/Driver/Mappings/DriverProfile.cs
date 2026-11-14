using Application.Features.Driver.DTOs;

namespace Application.Features.Driver.Mappings
{
    public class DriverProfile:Profile
    {
        public DriverProfile()
        {
            CreateMap<Domain.Drivers.Driver, DriverDto>().ReverseMap();
        }
    }
}