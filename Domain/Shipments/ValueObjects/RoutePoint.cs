using Domain.Shipments.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.ValueObjects
{
    public sealed record RoutePoint
    {
        public string Location { get; init; } = default!;
        public string Description { get; init; } = default!;
        public DateTime ArrivedAt { get; init; }
        public RoutePointType Type { get; init; }

        private RoutePoint() { } // EF Core

        private RoutePoint(string location, string description, DateTime arrivedAt, RoutePointType type)
        {
            Location = location;
            Description = description;
            ArrivedAt = arrivedAt;
            Type = type;
        }

        public static RoutePoint Create(
            string location,
            string description,
            DateTime arrivedAt,
            RoutePointType type)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location cannot be empty.", nameof(location));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.", nameof(description));
            if (arrivedAt == default)
                throw new ArgumentException("ArrivedAt must be a valid date.", nameof(arrivedAt));

            return new RoutePoint(location.Trim(), description.Trim(), arrivedAt, type);
        }

        public bool IsOrigin => Type == RoutePointType.Origin;
        public bool IsFinalDestination => Type == RoutePointType.Destination;
    }
}
