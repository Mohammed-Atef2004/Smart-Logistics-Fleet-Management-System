using Application.Features.Shift.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shift.Mappings
{
    public class ShiftProfile:Profile
    {
        public ShiftProfile()
        {
            CreateMap<Domain.Shifts.Shift, ShiftDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value));
                
        }
    }
}
