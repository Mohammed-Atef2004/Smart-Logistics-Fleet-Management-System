using Application.Features.Shift.DTOs;

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