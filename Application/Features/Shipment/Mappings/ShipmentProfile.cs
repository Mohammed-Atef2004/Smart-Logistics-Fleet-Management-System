using Application.Features.Shipment.DTOs;
using ShipmentAggregate = Domain.Shipments.Shipment;
using PackageEntity = Domain.Shipments.Package;

namespace Application.Features.Shipment.Mappings;

/// <summary>
/// Explicit mapping from the aggregate to the flat DTOs. Convention mapping
/// cannot see through <c>ShipmentId</c>/<c>PackageId</c> or the nested
/// <c>Tracking</c>/<c>DestinationAddress</c> value objects, and the old
/// <c>ReverseMap()</c> calls would have failed at runtime because the DTOs
/// cannot be turned back into a valid aggregate.
/// </summary>
public sealed class ShipmentProfile : Profile
{
    public ShipmentProfile()
    {
        CreateMap<ShipmentAggregate, ShipmentListDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.Value))
            .ForMember(d => d.TrackingNumber, o => o.MapFrom(s => s.Tracking.TrackingNumber))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Tracking.Status.ToString()))
            .ForMember(d => d.DestinationCity, o => o.MapFrom(s => s.DestinationAddress.City))
            .ForMember(d => d.CarrierName, o => o.MapFrom(s => s.Tracking.CarrierName))
            .ForMember(d => d.TotalPackages, o => o.MapFrom(s => s.TotalPackages))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt));

        CreateMap<ShipmentAggregate, ShipmentDetailsDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.Value))
            .ForMember(d => d.TrackingNumber, o => o.MapFrom(s => s.Tracking.TrackingNumber))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Tracking.Status.ToString()))
            .ForMember(d => d.StatusDescription, o => o.MapFrom(s => s.Tracking.StatusDescription))
            .ForMember(d => d.SenderId, o => o.MapFrom(s => s.SenderId))
            .ForMember(d => d.RecipientName, o => o.MapFrom(s => s.RecipientName))
            .ForMember(d => d.RecipientPhone, o => o.MapFrom(s => s.RecipientPhone))
            .ForMember(d => d.DestinationCity, o => o.MapFrom(s => s.DestinationAddress.City))
            .ForMember(d => d.DestinationAddress, o => o.MapFrom(s => s.DestinationAddress.FullAddress))
            .ForMember(d => d.CarrierName, o => o.MapFrom(s => s.Tracking.CarrierName))
            .ForMember(d => d.EstimatedDeliveryDate, o => o.MapFrom(s => s.Tracking.EstimatedDeliveryDate))
            .ForMember(d => d.TotalPackages, o => o.MapFrom(s => s.TotalPackages))
            .ForMember(d => d.TotalWeightKg, o => o.MapFrom(s => s.TotalWeightKg))
            .ForMember(d => d.TotalDeclaredValue, o => o.MapFrom(s => s.TotalDeclaredValue))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.DeliveredAt, o => o.MapFrom(s => s.DeliveredAt))
            .ForMember(d => d.Packages, o => o.MapFrom(s => s.Packages));

        CreateMap<PackageEntity, PackageDto>()
            .ForMember(d => d.Id, o => o.MapFrom(p => p.Id.Value))
            .ForMember(d => d.Description, o => o.MapFrom(p => p.Description))
            .ForMember(d => d.WeightKg, o => o.MapFrom(p => p.Weight.ToKilograms()))
            .ForMember(d => d.DeclaredValue, o => o.MapFrom(p => p.DeclaredValue))
            .ForMember(d => d.Currency, o => o.MapFrom(p => p.Currency))
            .ForMember(d => d.IsFragile, o => o.MapFrom(p => p.IsFragile))
            .ForMember(d => d.RequiresRefrigeration, o => o.MapFrom(p => p.RequiresRefrigeration));
    }
}
