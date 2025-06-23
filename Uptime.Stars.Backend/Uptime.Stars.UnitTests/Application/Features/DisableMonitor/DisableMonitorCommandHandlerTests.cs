using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Features.DisableMonitor;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.UnitTests.Application.Features.DisableMonitor;

public class DisableMonitorCommandHandlerTests
{
    private readonly IMonitorRepository _monitorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMonitorScheduler _scheduler;
    private readonly DisableMonitorCommandHandler _handler;

    public DisableMonitorCommandHandlerTests()
    {
        _monitorRepository = Substitute.For<IMonitorRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _scheduler = Substitute.For<IMonitorScheduler>();
        _handler = new DisableMonitorCommandHandler(_monitorRepository, _unitOfWork, _scheduler);
    }

    private static ComponentMonitor GetMonitor()
    {
        return ComponentMonitor.Create("Test", "", MonitorType.Https, "http://test.pe", DateTime.UtcNow, [], []);
    }

    [Fact]
    public async Task Handle_MonitorNotFound_ReturnsFailure()
    {
        // Arrange
        
        var command = new DisableMonitorCommand(Guid.NewGuid());
        
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
    public async Task Handle_MonitorAlreadyDisabled_ReturnsFailure()
    {
        // Arrange

        var monitor = GetMonitor();

        monitor.Disable();

        var command = new DisableMonitorCommand(Guid.NewGuid());
        
        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DisableMonitor.Handle");
        result.Error.Description.Should().Be("Monitor is already disabled");
    }

    [Fact]
    public async Task Handle_DisablesMonitor_AndReturnsSuccess()
    {
        // Arrange

        var monitor = GetMonitor();

        var command = new DisableMonitorCommand(monitor.Id);

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
        monitor.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldRemoveTaskFromScheduler()
    {

        // Arrange

        var monitor = GetMonitor();

        var command = new DisableMonitorCommand(monitor.Id);

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act

        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _scheduler.Received(1).RemoveAsync(monitor.Id, Arg.Any<CancellationToken>());
    }
}