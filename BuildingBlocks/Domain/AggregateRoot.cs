using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Domain
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IHasDomainEvents
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
