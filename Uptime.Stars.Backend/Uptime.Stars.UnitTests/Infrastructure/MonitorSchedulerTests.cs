using Hangfire;
using NSubstitute;
using Uptime.Stars.Infrastructure.Jobs;
using Uptime.Stars.Infrastructure.Services;

namespace Uptime.Stars.UnitTests.Infrastructure;

public class MonitorSchedulerTests
{
    private readonly IRecurringJobManager _jobManager = Substitute.For<IRecurringJobManager>();
    private readonly MonitorScheduler _scheduler;

    public MonitorSchedulerTests()
    {
        _scheduler = new MonitorScheduler(_jobManager);
    }

    [Fact]
    public async Task ScheduleAsync_CallsAddOrUpdate_WithCorrectParameters()
    {
        // Arrange

        var monitorId = Guid.NewGuid();
        int interval = 5;

        // Act
        
        await _scheduler.ScheduleAsync(monitorId, interval, CancellationToken.None);

        // Assert

        _jobManager.ReceivedWithAnyArgs(1).AddOrUpdate<MonitorJob>(
            $"monitor-{monitorId}",
            job => job.ExecuteAsync(monitorId, CancellationToken.None),
            "* */5 * * * *");
    }

    [Fact]
    public async Task ScheduleAsync_UsesMinimumIntervalOfOne()
    {
        // Arrange

        var monitorId = Guid.NewGuid();

        // Act
        
        await _scheduler.ScheduleAsync(monitorId, 0, CancellationToken.None);

        // Assert

        _jobManager.ReceivedWithAnyArgs(1).AddOrUpdate<MonitorJob>(
            $"monitor-{monitorId}",
            job => job.ExecuteAsync(monitorId, CancellationToken.None),
            "* */1 * * * *"
        );
    }
}