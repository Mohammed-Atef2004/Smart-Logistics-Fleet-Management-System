using Domain.Shipments.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.ValueObjects
{
    public sealed record TrackingInfo
    {
        public string TrackingNumber { get; init; } = default!;
        public ShipmentStatus Status { get; init; }
        public string StatusDescription { get; init; } = default!;
        public DateTime UpdatedAt { get; init; }
        public string? CarrierName { get; init; }
        public string? EstimatedDeliveryDate { get; init; }

        private TrackingInfo() { } // EF Core

        private TrackingInfo(
            string trackingNumber,
            ShipmentStatus status,
            string statusDescription,
            DateTime updatedAt,
            string? carrierName,
            string? estimatedDeliveryDate)
        {
            TrackingNumber = trackingNumber;
            Status = status;
            StatusDescription = statusDescription;
            UpdatedAt = updatedAt;
            CarrierName = carrierName;
            EstimatedDeliveryDate = estimatedDeliveryDate;
        }

        public static TrackingInfo Create(
            string trackingNumber,
            ShipmentStatus status,
            string statusDescription,
            string? carrierName = null,
            string? estimatedDeliveryDate = null)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("TrackingNumber cannot be empty.", nameof(trackingNumber));
            if (string.IsNullOrWhiteSpace(statusDescription))
                throw new ArgumentException("StatusDescription cannot be empty.", nameof(statusDescription));

            return new TrackingInfo(
                trackingNumber, status, statusDescription,
                DateTime.UtcNow, carrierName, estimatedDeliveryDate);
        }

        /// <summary>
        /// Functional update — returns new VO with updated status. Old one discarded.
        /// </summary>
        public TrackingInfo WithStatus(ShipmentStatus newStatus, string description) =>
            this with { Status = newStatus, StatusDescription = description, UpdatedAt = DateTime.UtcNow };

        public TrackingInfo WithCarrier(string carrierName, string? estimatedDeliveryDate = null) =>
            this with
            {
                CarrierName = carrierName,
                EstimatedDeliveryDate = estimatedDeliveryDate,
                UpdatedAt = DateTime.UtcNow
            };

        public bool IsTerminal =>
            Status is ShipmentStatus.Delivered or ShipmentStatus.Cancelled or ShipmentStatus.Returned;
    }
}
