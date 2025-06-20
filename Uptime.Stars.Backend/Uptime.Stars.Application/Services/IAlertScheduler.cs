namespace Uptime.Stars.Application.Services;
public interface IAlertScheduler
{
    Task ScheduleDownAlertAsync(Guid monitorId, int alertDelay, int cycle, CancellationToken cancellationToken = default);
    Task ScheduleUpAlertAsync(Guid monitorId, CancellationToken cancellationToken = default);
}
