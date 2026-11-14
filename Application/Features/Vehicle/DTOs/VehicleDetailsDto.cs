namespace Application.Features.Vehicle.DTOs
{
    public record VehicleDetailsDto(
        Guid Id,
        string PlateNumber,
        string Status,
        string Model,
        int Year,
        DateTime? LastMaintenanceDate);
}