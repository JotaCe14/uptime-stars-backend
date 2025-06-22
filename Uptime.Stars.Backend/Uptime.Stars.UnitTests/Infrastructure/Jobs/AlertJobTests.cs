using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;
using Uptime.Stars.Infrastructure.Jobs;

namespace Uptime.Stars.UnitTests.Infrastructure.Jobs
{
    public class AlertJobTests
    {
        private readonly IMonitorRepository _monitorRepository = Substitute.For<IMonitorRepository>();
        private readonly IEventRepository _eventRepository = Substitute.For<IEventRepository>();
        private readonly IAlertService _alertService = Substitute.For<IAlertService>();
        private readonly IAlertScheduler _alertScheduler = Substitute.For<IAlertScheduler>();
        private readonly AlertJob _job;

        public AlertJobTests()
        {
            _job = new AlertJob(_monitorRepository, _eventRepository, _alertService, _alertScheduler);
        }

        private static ComponentMonitor GetMonitor(int alertResendCycles = 0)
        {
            return ComponentMonitor.Create(
                "Monitor",
                "Desc",
                Domain.Enums.MonitorType.Https,
                "https://test.com",
                DateTime.UtcNow,
                [],
                [],
                timeoutInMilliseconds: 1000,
                alertResendCycles: alertResendCycles
            );
        }

        private static Event GetEvent(bool isUp)
        {
            return Event.Create(Guid.NewGuid(), DateTime.UtcNow, isUp, false, "msg", 100);
        }

        [Fact]
        public async Task AlertDownAsync_DoesNothing_WhenMonitorNotFound()
        {
            // Arrange

            _monitorRepository
                .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .ReturnsNullForAnyArgs();

            // Act

            await _job.AlertDownAsync(Guid.NewGuid(), 0, CancellationToken.None);

            // Assert

            await _eventRepository.DidNotReceiveWithAnyArgs().GetLastByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
            await _alertService.DidNotReceiveWithAnyArgs().SendAlertAsync(Arg.Any<ComponentMonitor>(), Arg.Any<Event>(), Arg.Any<CancellationToken>());
            await _alertScheduler.DidNotReceiveWithAnyArgs().ScheduleDownAlertAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AlertDownAsync_DoesNothing_WhenMonitorIsInactive()
        {
            // Arrange

            var monitor = GetMonitor();

            monitor.Disable();

            _monitorRepository
                .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .ReturnsNullForAnyArgs();

            // Act

            await _job.AlertDownAsync(Guid.NewGuid(), 0, CancellationToken.None);

            // Assert

            await _eventRepository.DidNotReceiveWithAnyArgs().GetLastByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
            await _alertService.DidNotReceiveWithAnyArgs().SendAlertAsync(Arg.Any<ComponentMonitor>(), Arg.Any<Event>(), Arg.Any<CancellationToken>());
            await _alertScheduler.DidNotReceiveWithAnyArgs().ScheduleDownAlertAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AlertDownAsync_DoesNothing_WhenNoLastEvent()
        {
            // Arrange

            var monitor = GetMonitor();

            _monitorRepository
                .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(monitor);

            _eventRepository
                .GetLastByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs([]);

            // Act

            await _job.AlertDownAsync(Guid.NewGuid(), 0, CancellationToken.None);

            // Assert

            await _alertService.DidNotReceiveWithAnyArgs().SendAlertAsync(Arg.Any<ComponentMonitor>(), Arg.Any<Event>(), Arg.Any<CancellationToken>());
            await _alertScheduler.DidNotReceiveWithAnyArgs().ScheduleDownAlertAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AlertDownAsync_DoesNothing_WhenLastEventIsUp()
        {
            // Arrange

            var monitor = GetMonitor();

            var lastEvent = GetEvent(isUp: true);

            _monitorRepository
                .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(monitor);

            _eventRepository
                .GetLastByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs([lastEvent]);

            // Act

            await _job.AlertDownAsync(Guid.NewGuid(), 0, CancellationToken.None);

            // Assert

            await _alertService.DidNotReceiveWithAnyArgs().SendAlertAsync(Arg.Any<ComponentMonitor>(), Arg.Any<Event>(), Arg.Any<CancellationToken>());
            await _alertScheduler.DidNotReceiveWithAnyArgs().ScheduleDownAlertAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(1, 4)]
        [InlineData(3, 4)]
        public async Task AlertDownAsync_SendsAlertAndSchedules_WhenCycleLessThanResendCycles(int currentCycle, int alertResendCycles)
        {
            // Arrange

            var monitor = GetMonitor(alertResendCycles: alertResendCycles);

            var lastEvent = GetEvent(isUp: false);

            _monitorRepository
                .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(monitor);

            _eventRepository
                .GetLastByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs([lastEvent]);

            // Act

            await _job.AlertDownAsync(monitor.Id, currentCycle, CancellationToken.None);

            // Assert

            await _alertService.Received(1).SendAlertAsync(monitor, lastEvent, Arg.Any<CancellationToken>());
            await _alertScheduler.Received(1).ScheduleDownAlertAsync(monitor.Id, monitor.AlertDelayMinutes, currentCycle + 1, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AlertDownAsync_SendsAlertAndDoesNotSchedule_WhenCycleEqualsResendCycles()
        {
            // Arrange

            var monitor = GetMonitor(alertResendCycles: 1);

            var lastEvent = GetEvent(isUp: false);

            _monitorRepository
                .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(monitor);

            _eventRepository
                .GetLastByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs([lastEvent]);

            // Act

            await _job.AlertDownAsync(monitor.Id, 1, CancellationToken.None);

            // Assert

            await _alertService.Received(1).SendAlertAsync(monitor, lastEvent, Arg.Any<CancellationToken>());
            await _alertScheduler.DidNotReceiveWithAnyArgs().ScheduleDownAlertAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        }


        [Fact]
        public async Task AlertUpAsync_DoesNothing_WhenMonitorNotFound()
        {
            // Arrange

            _monitorRepository
                .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .ReturnsNullForAnyArgs();

            // Act

            await _job.AlertUpAsync(Guid.NewGuid(), CancellationToken.None);

            // Assert

            await _eventRepository.DidNotReceiveWithAnyArgs().GetLastByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
            await _alertService.DidNotReceiveWithAnyArgs().SendAlertAsync(Arg.Any<ComponentMonitor>(), Arg.Any<Event>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AlertUpAsync_DoesNothing_WhenMonitorIsInactive()
        {
            // Arrange

            var monitor = GetMonitor();

            monitor.Disable();

            _monitorRepository
                .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .ReturnsNullForAnyArgs();

            // Act

            await _job.AlertUpAsync(monitor.Id, CancellationToken.None);

            // Assert

            await _eventRepository.DidNotReceiveWithAnyArgs().GetLastByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
            await _alertService.DidNotReceiveWithAnyArgs().SendAlertAsync(Arg.Any<ComponentMonitor>(), Arg.Any<Event>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AlertUpAsync_DoesNothing_WhenNoLastEvent()
        {
            // Arrange

            var monitor = GetMonitor();

            _monitorRepository
                .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(monitor);

            _eventRepository
                .GetLastByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs([]);

            // Act

            await _job.AlertUpAsync(Guid.NewGuid(), CancellationToken.None);

            // Assert

            await _alertService.DidNotReceiveWithAnyArgs().SendAlertAsync(Arg.Any<ComponentMonitor>(), Arg.Any<Event>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AlertUpAsync_DoesNothing_WhenLastEventIsDown()
        {
            // Arrange

            var monitor = GetMonitor();

            var lastEvent = GetEvent(isUp: false);

            _monitorRepository
                .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(monitor);

            _eventRepository
                .GetLastByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs([lastEvent]);

            // Act

            await _job.AlertUpAsync(Guid.NewGuid(), CancellationToken.None);

            // Assert

            await _alertService.DidNotReceiveWithAnyArgs().SendAlertAsync(Arg.Any<ComponentMonitor>(), Arg.Any<Event>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AlertUpAsync_SendsAlert_WhenLastEventIsUp()
        {
            // Arrange

            var monitor = GetMonitor();

            var lastEvent = GetEvent(isUp: true);

            _monitorRepository
                .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(monitor);

            _eventRepository
                .GetLastByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs([lastEvent]);

            // Act

            await _job.AlertUpAsync(Guid.NewGuid(), CancellationToken.None);

            // Assert

            await _alertService.Received(1).SendAlertAsync(monitor, lastEvent, Arg.Any<CancellationToken>());
        }
    }
}
