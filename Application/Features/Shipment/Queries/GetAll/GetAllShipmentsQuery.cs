using Application.Features.Shipment.DTOs;
using Domain.Shipments.Enums;

namespace Application.Features.Shipment.Queries.GetAll;

/// <summary>Lists shipments, optionally filtered by status.</summary>
public sealed record GetAllShipmentsQuery(ShipmentStatus? Status = null)
    : IRequest<Result<List<ShipmentListDto>>>;
