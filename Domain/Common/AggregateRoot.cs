using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public abstract class AggregateRoot
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();
        // Expose domain events as a read-only collection
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        // Protected to allow only derived classes to add domain events
        protected void AddDomainEvent(DomainEvent domainEvent)=>_domainEvents.Add(domainEvent);
        //public method to clear domain events, because after dispatching them, they should be cleared,
        //the dispatching logic will be handled in the infrastructure layer with the help of a mediator so we need
        //it to be public to hanlde the clearing logic
        public void ClearDomainEvents() => _domainEvents.Clear();



    }
}
