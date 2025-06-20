using Hangfire;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Infrastructure.Jobs;

namespace Uptime.Stars.Infrastructure.Services;
internal sealed class MonitorScheduler(IRecurringJobManager jobManager) : IMonitorScheduler
{
    public Task RemoveAsync(Guid monitorId, CancellationToken cancellationToken = default)
    {
        var jobId = $"monitor-{monitorId}";

        jobManager.RemoveIfExists(jobId);

        return Task.CompletedTask;
    }

    public Task ScheduleAsync(Guid monitorId, int intervalMinutes, CancellationToken cancellationToken = default)
    {
        var intervalInMinutes = intervalMinutes > 0 ? intervalMinutes : 1;

        var jobId = $"monitor-{monitorId}";

        jobManager.AddOrUpdate<MonitorJob>(
           jobId,
           job => job.ExecuteAsync(monitorId, CancellationToken.None),
           $"*/{intervalInMinutes} * * * *"
        );

        return Task.CompletedTask;
    }
}