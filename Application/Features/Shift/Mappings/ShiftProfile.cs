using Application.Features.Shift.DTOs;

namespace Application.Features.Shift.Mappings;

/// <summary>
/// Explicit mapping from the aggregate to the flat DTO. Convention mapping
/// cannot see through <c>ShiftId</c>/<c>DriverId</c>, and the property names
/// differ (ShiftStart/ShiftEnd vs Start/End), so every constructor argument
/// is mapped by hand.
/// </summary>
public sealed class ShiftProfile : Profile
{
    public ShiftProfile()
    {
        CreateMap<Domain.Shifts.Shift, ShiftDto>()
            .ForCtorParam(nameof(ShiftDto.Id), opt => opt.MapFrom(src => src.Id.Value))
            .ForCtorParam(nameof(ShiftDto.DriverId), opt => opt.MapFrom(src => src.DriverId.Value))
            .ForCtorParam(nameof(ShiftDto.Start), opt => opt.MapFrom(src => src.ShiftStart))
            .ForCtorParam(nameof(ShiftDto.End), opt => opt.MapFrom(src => src.ShiftEnd))
            .ForCtorParam(nameof(ShiftDto.Status), opt => opt.MapFrom(src => src.Status.ToString()))
            .ForCtorParam(nameof(ShiftDto.StartedAtUtc), opt => opt.MapFrom(src => src.StartedAtUtc))
            .ForCtorParam(nameof(ShiftDto.EndedAtUtc), opt => opt.MapFrom(src => src.EndedAtUtc));
    }
}
