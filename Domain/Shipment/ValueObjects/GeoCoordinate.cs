using Domain.Common;
using Domain.Common.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Domain.Shipment.ValueObjects
{
    [Owned]
    public class GeoCoordinate : ValueObject
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GeoCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
        private GeoCoordinate() { } // EF Core
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }
    }
}
