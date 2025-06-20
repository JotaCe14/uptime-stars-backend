namespace Uptime.Stars.Application.Services;
public interface IMonitorScheduler
{
    Task ScheduleAsync(Guid monitorId, int intervalMinutes, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid monitorId, CancellationToken cancellationToken = default);
}