namespace Application.Features.Shipment.Commands.UpdateDeliveryAddress;

/// <summary>
/// Replaces the delivery address while the shipment is still editable. The
/// address is supplied as primitives because a domain value object cannot be
/// bound from JSON.
/// </summary>
public sealed record UpdateShipmentDeliveryAddressCommand(
    Guid ShipmentId,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    string? ApartmentUnit = null) : IRequest<Result>;
