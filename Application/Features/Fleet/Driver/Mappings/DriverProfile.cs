using Application.Features.Fleet.Driver.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Driver.Mappings
{
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<Domain.Fleet.Entities.Driver, DriverDto>().ReverseMap();
        }
    }
}