using Application.Features.Driver.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
