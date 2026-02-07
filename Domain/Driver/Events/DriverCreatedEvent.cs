using Domain.Common;

namespace Domain.Driver.Events
{
    public class DriverCreatedEvent : DomainEvent
    {
    
        public string FullName { get; }
        public Guid Id { get; }
        public string LicenceNumber { get; }


        public DriverCreatedEvent(Guid Id ,string fullName, string  licencenumber)
        {
            this.Id = Id;
            FullName = fullName;
            LicenceNumber = licencenumber;
        }
    }

}