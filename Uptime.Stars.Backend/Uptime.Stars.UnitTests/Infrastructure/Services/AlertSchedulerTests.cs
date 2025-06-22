using Hangfire;
using NSubstitute;
using Uptime.Stars.Infrastructure.Jobs;
using Uptime.Stars.Infrastructure.Services;

namespace Uptime.Stars.UnitTests.Infrastructure.Services;

public class AlertSchedulerTests
{
    private readonly IBackgroundJobClient _jobClient = Substitute.For<IBackgroundJobClient>();
    private readonly AlertScheduler _scheduler;

    public AlertSchedulerTests()
    {
        _scheduler = new AlertScheduler(_jobClient);
    }

    [Fact]
    public async Task ScheduleDownAlertAsync_CallsSchedule_WithCorrectParameters()
    {
        // Arrange

        var monitorId = Guid.NewGuid();
        int alertDelay = 10;
        int cycle = 2;

        // Act

        await _scheduler.ScheduleDownAlertAsync(monitorId, alertDelay, cycle, CancellationToken.None);

        // Assert

        _jobClient.ReceivedWithAnyArgs(1).Schedule<AlertJob>(
            job => job.AlertDownAsync(monitorId, cycle, CancellationToken.None),
            TimeSpan.FromMinutes(alertDelay)
        );
    }

    [Fact]
    public async Task ScheduleUpAlertAsync_CallsSchedule_WithCorrectParameters()
    {
        // Arrange

        var monitorId = Guid.NewGuid();

        // Act
        
        await _scheduler.ScheduleUpAlertAsync(monitorId);

        // Assert
        
        _jobClient.ReceivedWithAnyArgs(1).Schedule<AlertJob>(
            job => job.AlertUpAsync(monitorId, CancellationToken.None),
            TimeSpan.FromSeconds(1)
        );
    }
}
