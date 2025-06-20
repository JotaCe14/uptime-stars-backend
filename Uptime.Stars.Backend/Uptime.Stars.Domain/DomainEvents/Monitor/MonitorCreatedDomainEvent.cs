using Uptime.Stars.Domain.Core.Events;

namespace Uptime.Stars.Domain.DomainEvents.Monitor;
public sealed record MonitorCreatedDomainEvent(Guid MonitorId, int IntervalMinutes) : IDomainEvent;