using Hangfire;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Infrastructure.Jobs;

namespace Uptime.Stars.Infrastructure.Services;
internal sealed class AlertScheduler(IBackgroundJobClient jobClient) : IAlertScheduler
{
    public Task ScheduleDownAlertAsync(Guid monitorId, int alertDelay, int cycle, CancellationToken cancellationToken = default)
    {
        jobClient.Schedule<AlertJob>(
            job => job.AlertDownAsync(monitorId, cycle, CancellationToken.None),
            TimeSpan.FromMinutes(alertDelay)
        );

        return Task.CompletedTask;
    }

    public Task ScheduleUpAlertAsync(Guid monitorId, CancellationToken cancellationToken = default)
    {
        jobClient.Schedule<AlertJob>(
            job => job.AlertUpAsync(monitorId, CancellationToken.None),
            TimeSpan.FromSeconds(1)
        );

        return Task.CompletedTask;
    }
}