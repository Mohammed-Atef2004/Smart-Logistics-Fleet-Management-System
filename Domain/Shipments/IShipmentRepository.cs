using Application.Features.Shipments.ValueObjects;
using Domain.Interfaces.Repositories;
using Domain.Shipments.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments
{
    public  interface IShipmentRepository: IGenericRepository<Shipment>
    {
        Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default);
        Task<bool> TrackingNumberExistsAsync(string trackingNumber, CancellationToken ct = default);

    }

    
}
