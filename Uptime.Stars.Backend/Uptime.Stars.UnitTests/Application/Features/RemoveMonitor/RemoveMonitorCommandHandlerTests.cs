using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Uptime.Stars.Application.Features.RemoveMonitor;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.UnitTests.Application.Features.RemoveMonitor;

public class RemoveMonitorCommandHandlerTests
{
    private readonly IMonitorRepository _monitorRepository;
    private readonly IMonitorScheduler _scheduler;
    private readonly RemoveMonitorCommandHandler _handler;

    public RemoveMonitorCommandHandlerTests()
    {
        _monitorRepository = Substitute.For<IMonitorRepository>();
        _scheduler = Substitute.For<IMonitorScheduler>();
        _handler = new RemoveMonitorCommandHandler(_monitorRepository, _scheduler);
    }

    private static ComponentMonitor GetMonitor()
    {
        return ComponentMonitor.Create("Test", "", MonitorType.Https, "http://test.pe", DateTime.UtcNow, [], []);
    }

    [Fact]
    public async Task Handle_MonitorNotFound_ReturnsFailure()
    {
        // Arrange

        var command = new RemoveMonitorCommand(Guid.NewGuid());
        
        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsNullForAnyArgs();

        // Act
        
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("EnableMonitor.Handle");
        result.Error.Description.Should().Be("Monitor not found");
    }

    [Fact]
    public async Task Handle_MonitorFound_RemovesFromScheduler()
    {
        // Arrange

        var monitor = GetMonitor();
        
        var command = new RemoveMonitorCommand(monitor.Id);

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        // Act
        
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        
        await _scheduler.Received(1).RemoveAsync(monitor.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MonitorFound_DeletesMonitor()
    {
        // Arrange

        var monitor = GetMonitor();

        var command = new RemoveMonitorCommand(monitor.Id);

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        // Act

        await _handler.Handle(command, CancellationToken.None);

        // Assert

        await _monitorRepository.Received(1).DeleteAsync(monitor.Id, Arg.Any<CancellationToken>());
    }
}
