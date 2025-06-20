using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Infrastructure.Jobs;
internal sealed class AlertJob(
    IMonitorRepository monitorRepository, 
    IEventRepository eventRepository,
    IAlertService alertService,
    IAlertScheduler alertScheduler)
{
    public async Task AlertDownAsync(Guid monitorId, int cycle, CancellationToken cancellationToken = default)
    {
        var monitor = await monitorRepository.GetByIdAsync(monitorId, cancellationToken);

        if (monitor is null || !monitor.IsActive)
        {
            return;
        }

        var lastEvents = await eventRepository.GetLastByIdAsync(monitorId, 1, cancellationToken);

        var lastEvent = lastEvents.FirstOrDefault();

        if (lastEvent is null || lastEvent.IsUp)
        {
            return;
        }
        
        await alertService.SendAlertAsync(monitor, lastEvent, cancellationToken);

        if (cycle < monitor.AlertResendCycles)
        {
            await alertScheduler.ScheduleDownAlertAsync(monitorId, monitor.AlertDelayMinutes, cycle + 1, cancellationToken);
        }
    }
    public async Task AlertUpAsync(Guid monitorId, CancellationToken cancellationToken = default)
    {
        var monitor = await monitorRepository.GetByIdAsync(monitorId, cancellationToken);

        if (monitor is null || !monitor.IsActive)
        {
            return;
        }

        var lastEvents = await eventRepository.GetLastByIdAsync(monitorId, 1, cancellationToken);

        var lastEvent = lastEvents.FirstOrDefault();

        if (lastEvent is null || !lastEvent.IsUp)
        {
            return;
        }

        await alertService.SendAlertAsync(monitor, lastEvent, cancellationToken);
    }
}