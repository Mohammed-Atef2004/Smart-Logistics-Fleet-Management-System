using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using Domain.Shipments.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
