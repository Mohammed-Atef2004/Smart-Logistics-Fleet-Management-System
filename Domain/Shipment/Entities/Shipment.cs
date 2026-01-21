using Domain.Common;
using Domain.Shipment.Enums;
using Domain.Shipment.Events;
using Domain.Shipment.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Entities
{
    public class Shipment : AggregateRoot
    {
        public string TrackingNumber { get; private set; }
        public ShipmentStatus Status { get; private set; }
        public Route Route { get; private set; }

        private readonly List<Package> _packages = new();
        public IReadOnlyCollection<Package> Packages => _packages.AsReadOnly();

        private readonly List<TrackingUpdate> _trackingUpdates = new();
        public IReadOnlyCollection<TrackingUpdate> TrackingUpdates => _trackingUpdates.AsReadOnly();

        private Shipment() { } // EF

        public Shipment(string trackingNumber, Route route)
        {
            TrackingNumber = trackingNumber;
            Route = route;
            Status = ShipmentStatus.Created;

            AddDomainEvent(new ShipmentCreatedEvent(Id));
        }

        public void AddPackage(Package package)
        {
            _packages.Add(package);
        }

        public void UpdateStatus(ShipmentStatus newStatus)
        {
            CheckRule(new ShipmentCannotBeDeliveredTwiceRule(Status));

            Status = newStatus;
            AddDomainEvent(new ShipmentStatusUpdatedEvent(Id, newStatus));
        }

        public void AddTrackingUpdate(string location)
        {
            _trackingUpdates.Add(new TrackingUpdate(Id, location));
        }
    }

}
