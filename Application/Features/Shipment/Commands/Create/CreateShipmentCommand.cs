using Domain.Shipments.Enums;

namespace Application.Features.Shipment.Commands.Create;

/// <summary>
/// Creates a shipment. The address is supplied as primitives so the request
/// binds from JSON; the handler builds the value objects and returns their
/// validation errors instead of throwing.
/// </summary>
public sealed record CreateShipmentCommand(
    string SenderId,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    string? ApartmentUnit,
    string TrackingNumber,
    ShipmentPriority Priority = ShipmentPriority.Standard,
    string? RecipientName = null,
    string? RecipientPhone = null,
    string? SpecialInstructions = null) : IRequest<Result<Guid>>;
