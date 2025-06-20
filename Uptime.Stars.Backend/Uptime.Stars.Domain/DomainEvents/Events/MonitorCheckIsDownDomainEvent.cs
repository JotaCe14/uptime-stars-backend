using Uptime.Stars.Domain.Core.Events;
using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Domain.DomainEvents.Events;
public record MonitorCheckIsDownDomainEvent(Event Event) : IDomainEvent;