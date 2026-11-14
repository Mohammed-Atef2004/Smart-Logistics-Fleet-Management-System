using Domain.Shipments.ValueObjects;
using Domain.Shipments.Enums;

namespace Application.Features.Shipment.Commands.Create
{
    public sealed record CreateShipmentCommand(
     string SenderId,
     string Street,
     string City,
     string State,
     string ZipCode,
     string Country,
     string? ApartmentUnit,
     string TrackingNumber,
     ShipmentPriority Priority,
     string? RecipientName,
     string? RecipientPhone,
     string? SpecialInstructions
 ) : IRequest<Result<ShipmentId>>;
}