namespace Application.Features.Shipment.Commands.Dispatch;

/// <summary>Dispatches a shipment that has at least one package and a carrier.</summary>
public sealed record DispatchCommand(Guid ShipmentId) : IRequest<Result>;
