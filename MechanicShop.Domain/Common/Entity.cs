namespace MechanicShop.Domain.Common
{
    // Base abstract class for all entities in the domain
    public abstract class Entity
    {
        // Unique identifier for the entity
        public Guid Id { get; }

        // A list to store domain events associated with this entity
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();

        // Parameterless constructor for Entity Framework (EF) to materialize entities
        protected Entity() { }

        // Constructor that allows setting the Id, generates a new Guid if the given one is empty
        protected Entity(Guid id)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
        }

        // Adds a domain event to the entity
        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        // Removes a specific domain event from the entity
        public void RemoveDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        // Clears all domain events from the entity
        public void ClearDomainEvent()
        {
            _domainEvents.Clear();
        }
    }
}
