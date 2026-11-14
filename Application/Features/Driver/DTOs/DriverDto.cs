using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.DTOs
{
    public record DriverDto(
    DriverId Id,
    string FullName,
    string Status,
    double Rating
);
}