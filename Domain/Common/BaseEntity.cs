using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }

        // List to hold domain events that occurred on this entity
        private readonly List<DomainEvent> _domainEvents = new();

        // Standard EF Core doesn't need to map this to a database column
        [NotMapped]
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        // Method to register a new event
        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        // Method to remove a specific event
        public void RemoveDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        // Method to clear all events after they are dispatched by the Interceptor
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}