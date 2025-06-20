using Uptime.Stars.Domain.Core.Events;

namespace Uptime.Stars.Domain.Core.Primitives;

public abstract class AggregateRoot : Entity
{
    protected AggregateRoot()
    {
    }

    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
