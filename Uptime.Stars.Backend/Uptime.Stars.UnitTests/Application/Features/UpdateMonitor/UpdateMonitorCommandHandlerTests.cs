using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Features.UpdateMonitor;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.UnitTests.Application.Features.UpdateMonitor;

public class UpdateMonitorCommandHandlerTests
{
    private readonly IMonitorRepository _monitorRepository;
    private readonly IMonitorScheduler _monitorScheduler;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateMonitorCommandHandler _handler;

    public UpdateMonitorCommandHandlerTests()
    {
        _monitorRepository = Substitute.For<IMonitorRepository>();
        _monitorScheduler = Substitute.For<IMonitorScheduler>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new UpdateMonitorCommandHandler(_monitorRepository, _monitorScheduler, _unitOfWork);
    }

    private static ComponentMonitor GetMonitor()
    {
        return ComponentMonitor.Create(
            "Test", 
            "Desc", 
            MonitorType.Https, 
            "http://test.pe", 
            DateTime.UtcNow, 
            [], 
            []
        );
    }

    private static UpdateMonitorCommand GetCommand(Guid id, int interval = 5)
    {
        return new UpdateMonitorCommand(
            id,
            "Updated Name",
            "Updated Desc",
            Guid.NewGuid(),
            MonitorType.Https,
            "http://updated.pe",
            interval,
            10000,
            [],
            TextSearchMode.Include,
            "",
            [],
            "",
            0,
            3
        );
    }

    [Fact]
    public async Task Handle_MonitorNotFound_ReturnsFailure()
    {
        // Arrange

        var command = GetCommand(Guid.NewGuid());
        
        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsNullForAnyArgs();

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DisableMonitor.Handle");
        result.Error.Description.Should().Be("Monitor not found");
    }

    [Fact]
    public async Task Handle_UpdatesMonitor_AndReturnsSuccess()
    {
        // Arrange
        
        var monitor = GetMonitor();
        
        var command = GetCommand(monitor.Id);

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        
        result.IsSuccess.Should().BeTrue();
        monitor.Name.Should().Be("Updated Name");
        monitor.Description.Should().Be("Updated Desc");
        monitor.Target.Should().Be("http://updated.pe");
    }

    [Fact]
    public async Task Handle_IntervalChanged_ShouldRescheduleMonitor()
    {
        // Arrange

        var monitor = GetMonitor();
        
        var command = GetCommand(monitor.Id, interval: 10);

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act

        await _handler.Handle(command, CancellationToken.None);

        // Assert
        
        await _monitorScheduler.Received(1).ScheduleAsync(monitor.Id, 10, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_IntervalNotChanged_ShouldNotRescheduleMonitor()
    {
        // Arrange

        var monitor = GetMonitor();
        
        var command = GetCommand(monitor.Id, interval: 1);

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act

        await _handler.Handle(command, CancellationToken.None);

        // Assert
        
        await _monitorScheduler.DidNotReceive().ScheduleAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }
}
