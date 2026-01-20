using Domain.Common;

namespace Domain.Fleet.Events
{
    public class DriverCreatedEvent : DomainEvent
    {
    
        public string FullName { get; }
        public Guid Id { get; }
        public string LicenceNumber { get; }


        public DriverCreatedEvent(Guid Id ,string fullName, string  licencenumber)
        {
            this.Id = Id;
            this.FullName = fullName;
            this.LicenceNumber = licencenumber;
        }
    }

}