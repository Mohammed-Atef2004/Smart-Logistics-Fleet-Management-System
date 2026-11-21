using Application.Features.Driver.DTOs;

namespace Application.Features.Driver.Mappings;

public sealed class DriverProfile : Profile
{
    public DriverProfile()
    {
        // Explicit mapping: the old profile relied on conventions, which
        // cannot map DriverRating -> double or DriverId -> Guid. That
        // made GET /api/drivers/{id} throw at runtime.
        CreateMap<Domain.Drivers.Driver, DriverDto>()
            .ForCtorParam(nameof(DriverDto.Id), opt => opt.MapFrom(src => src.Id.Value))
            .ForCtorParam(nameof(DriverDto.FullName), opt => opt.MapFrom(src => src.FullName))
            .ForCtorParam(nameof(DriverDto.Status), opt => opt.MapFrom(src => src.Status.ToString()))
            .ForCtorParam(nameof(DriverDto.Rating), opt => opt.MapFrom(src => src.Rating.Value))
            .ForCtorParam(nameof(DriverDto.RatingCount), opt => opt.MapFrom(src => src.Rating.Count))
            .ForCtorParam(nameof(DriverDto.LicenseNumber), opt => opt.MapFrom(src => src.License.LicenseNumber))
            .ForCtorParam(nameof(DriverDto.LicenseExpiryDate), opt => opt.MapFrom(src => src.License.ExpiryDate))
            .ForCtorParam(nameof(DriverDto.LicenseCategory), opt => opt.MapFrom(src => src.License.Category))
            .ForCtorParam(nameof(DriverDto.CurrentShiftId),
                opt => opt.MapFrom(src => src.CurrentShiftId != null ? src.CurrentShiftId.Value : (Guid?)null));
    }
}
