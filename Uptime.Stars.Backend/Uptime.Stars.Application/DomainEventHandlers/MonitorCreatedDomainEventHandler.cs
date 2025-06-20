using MediatR;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.DomainEvents.Monitor;

namespace Uptime.Stars.Application.DomainEventHandlers;
internal sealed class MonitorCreatedDomainEventHandler(IMonitorScheduler scheduler) : INotificationHandler<MonitorCreatedDomainEvent>
{
    public async Task Handle(MonitorCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await scheduler.ScheduleAsync(notification.MonitorId, notification.IntervalMinutes, cancellationToken);
    }
}