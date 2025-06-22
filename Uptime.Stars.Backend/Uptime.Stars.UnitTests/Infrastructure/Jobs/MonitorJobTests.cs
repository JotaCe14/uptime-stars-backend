using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;
using Uptime.Stars.Domain.Repositories;
using Uptime.Stars.Infrastructure.Jobs;
using Uptime.Stars.Infrastructure.Strategies;

namespace Uptime.Stars.UnitTests.Infrastructure.Jobs;

public class MonitorJobTests
{
    private readonly ICheckStrategyFactory _strategyFactory = Substitute.For<ICheckStrategyFactory>();
    private readonly ICheckStrategy _strategy = Substitute.For<ICheckStrategy>();
    private readonly IDateTime _dateTime = Substitute.For<IDateTime>();
    private readonly IMonitorRepository _monitorRepository = Substitute.For<IMonitorRepository>();
    private readonly IEventRepository _eventRepository = Substitute.For<IEventRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAlertScheduler _alertScheduler = Substitute.For<IAlertScheduler>();
    private readonly MonitorJob _job;

    public MonitorJobTests()
    {
        _job = new MonitorJob(_strategyFactory, _dateTime, _monitorRepository, _eventRepository, _unitOfWork, _alertScheduler);
    }

    private static ComponentMonitor GetMonitor(
        string? expectedText = null,
        TextSearchMode searchMode = TextSearchMode.Include,
        int timeout = 1000)
    {
        return ComponentMonitor.Create(
            "",
            "",
            MonitorType.Https,
            "https://test.com",
            DateTime.UtcNow,
            [],
            [],
            searchMode: searchMode,
            timeoutInMilliseconds: timeout,
            expectedText: expectedText);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNothing_WhenMonitorNotFound()
    {
        // Arrange

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsNullForAnyArgs();

        // Act

        await _job.ExecuteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert

        _strategyFactory.DidNotReceiveWithAnyArgs().GetStrategy(Arg.Any<MonitorType>());
        await _eventRepository.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<Event>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_DoesNothing_WhenMonitorIsInactive()
    {
        // Arrange

        var monitor = GetMonitor();

        monitor.Disable();

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        // Act

        await _job.ExecuteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert

        _strategyFactory.DidNotReceiveWithAnyArgs().GetStrategy(Arg.Any<MonitorType>());
        await _eventRepository.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<Event>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenStrategyNotFound()
    {
        // Arrange

        var monitor = GetMonitor();

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _strategyFactory
            .GetStrategy(Arg.Any<MonitorType>()
            ).ReturnsNullForAnyArgs();

        // Act

        var act = async () => await _job.ExecuteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No check strategy found for monitor type: Https");
    }

    [Fact]
    public async Task ExecuteAsync_AddsEventAndSaves_WhenMonitorIsActiveAndStrategyExists()
    {
        // Arrange

        var monitor = GetMonitor();

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _strategyFactory
            .GetStrategy(Arg.Any<MonitorType>())
            .ReturnsForAnyArgs(_strategy);

        _strategy
            .CheckAsync(Arg.Any<ComponentMonitor>(), Arg.Any<CancellationToken>())
            .Returns(new CheckResult(true));

        _dateTime
            .UtcNow
            .ReturnsForAnyArgs(DateTime.UtcNow);

        // Act

        await _job.ExecuteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert

        await _eventRepository.ReceivedWithAnyArgs(1).AddAsync(
            Arg.Any<Event>(),
            Arg.Any<CancellationToken>());

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_SchedulesDownAlert_WhenMonitorGoesDown()
    {
        // Arrange

        var monitor = GetMonitor();

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _strategyFactory
            .GetStrategy(Arg.Any<MonitorType>())
            .ReturnsForAnyArgs(_strategy);

        _strategy
            .CheckAsync(Arg.Any<ComponentMonitor>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(new CheckResult(false, "Error"));

        _dateTime
            .UtcNow
            .ReturnsForAnyArgs(DateTime.UtcNow);

        _eventRepository
            .IsFirstByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(false);

        // Act

        await _job.ExecuteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert

        await _alertScheduler.Received(1).ScheduleDownAlertAsync(monitor.Id, monitor.AlertDelayMinutes, 0, Arg.Any<CancellationToken>());
        await _alertScheduler.DidNotReceiveWithAnyArgs().ScheduleUpAlertAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_SchedulesUpAlert_WhenMonitorRecovers()
    {
        // Arrange

        var monitor = GetMonitor();

        monitor.Check(isUp: false);

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _strategyFactory
            .GetStrategy(Arg.Any<MonitorType>())
            .ReturnsForAnyArgs(_strategy);

        _strategy
            .CheckAsync(Arg.Any<ComponentMonitor>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(new CheckResult(true));

        _dateTime
            .UtcNow
            .ReturnsForAnyArgs(DateTime.UtcNow);

        _eventRepository
            .IsFirstByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(false);

        // Act

        await _job.ExecuteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert

        await _alertScheduler.DidNotReceiveWithAnyArgs().ScheduleDownAlertAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _alertScheduler.Received().ScheduleUpAlertAsync(monitor.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotScheduleAlerts_WhenLastStatusIsUpAndCheckIsUp()
    {
        // Arrange

        var monitor = GetMonitor();

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _strategyFactory
            .GetStrategy(Arg.Any<MonitorType>())
            .ReturnsForAnyArgs(_strategy);

        _strategy
            .CheckAsync(Arg.Any<ComponentMonitor>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(new CheckResult(true));

        _dateTime
            .UtcNow
            .ReturnsForAnyArgs(DateTime.UtcNow);

        _eventRepository
            .IsFirstByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(false);

        // Act

        await _job.ExecuteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert

        await _alertScheduler.DidNotReceiveWithAnyArgs().ScheduleDownAlertAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _alertScheduler.DidNotReceiveWithAnyArgs().ScheduleUpAlertAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotScheduleAlerts_WhenLastStatusIsDownAndCheckIsDown()
    {
        // Arrange

        var monitor = GetMonitor();

        monitor.Check(isUp: false);

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _strategyFactory
            .GetStrategy(Arg.Any<MonitorType>())
            .ReturnsForAnyArgs(_strategy);

        _strategy
            .CheckAsync(Arg.Any<ComponentMonitor>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(new CheckResult(false));

        _dateTime
            .UtcNow
            .ReturnsForAnyArgs(DateTime.UtcNow);

        _eventRepository
            .IsFirstByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(false);

        // Act

        await _job.ExecuteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert

        await _alertScheduler.DidNotReceiveWithAnyArgs().ScheduleDownAlertAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _alertScheduler.DidNotReceiveWithAnyArgs().ScheduleUpAlertAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_AddsImportantEvent_WhenEventIsDown()
    {
        // Arrange

        var monitor = GetMonitor();

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _strategyFactory
            .GetStrategy(Arg.Any<MonitorType>())
            .ReturnsForAnyArgs(_strategy);

        _strategy
            .CheckAsync(Arg.Any<ComponentMonitor>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(new CheckResult(false));

        _dateTime
            .UtcNow
            .ReturnsForAnyArgs(DateTime.UtcNow);

        _eventRepository
            .IsFirstByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(false);

        // Act

        await _job.ExecuteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert

        await _eventRepository
            .Received(1)
            .AddAsync(
                Arg.Is<Event>(e => e.IsImportant),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_AddsImportantEvent_WhenEventRecovers()
    {
        // Arrange

        var monitor = GetMonitor();

        monitor.Check(isUp: false);

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _strategyFactory
            .GetStrategy(Arg.Any<MonitorType>())
            .ReturnsForAnyArgs(_strategy);

        _strategy
            .CheckAsync(Arg.Any<ComponentMonitor>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(new CheckResult(true));

        _dateTime
            .UtcNow
            .ReturnsForAnyArgs(DateTime.UtcNow);

        _eventRepository
            .IsFirstByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(false);

        // Act

        await _job.ExecuteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert

        await _eventRepository
            .Received(1)
            .AddAsync(
                Arg.Is<Event>(e => e.IsImportant),
                Arg.Any<CancellationToken>());
    }



    [Fact]
    public async Task ExecuteAsync_AddsImportantEvent_WhenFirstEvent()
    {
        // Arrange

        var monitor = GetMonitor();

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _strategyFactory
            .GetStrategy(Arg.Any<MonitorType>())
            .ReturnsForAnyArgs(_strategy);

        _strategy
            .CheckAsync(Arg.Any<ComponentMonitor>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(new CheckResult(true));

        _dateTime
            .UtcNow
            .ReturnsForAnyArgs(DateTime.UtcNow);

        _eventRepository
            .IsFirstByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(true);

        // Act

        await _job.ExecuteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert

        await _eventRepository
            .Received(1)
            .AddAsync(
                Arg.Is<Event>(e => e.IsImportant),
                Arg.Any<CancellationToken>());
    }
}