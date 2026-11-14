using Application.Common.Interfaces;
using Domain.Shipments.ValueObjects;
using Domain.Shipments;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public sealed class ShipmentRepository :GenericRepository<Shipment>, IShipmentRepository
    {
    
        public ShipmentRepository(AppDbContext context):base(context)
        {
            }



        public async Task<Shipment?> GetByTrackingNumberAsync(
            string trackingNumber,
            CancellationToken ct = default) =>
            await _context.Shipments
                .AsSplitQuery()
                .FirstOrDefaultAsync(s => s.Tracking.TrackingNumber == trackingNumber, ct);

        public async Task<bool> TrackingNumberExistsAsync(string trackingNumber, CancellationToken ct = default) =>
            await _context.Shipments
                .AnyAsync(s => s.Tracking.TrackingNumber == trackingNumber, ct);

       

       
    }
}
