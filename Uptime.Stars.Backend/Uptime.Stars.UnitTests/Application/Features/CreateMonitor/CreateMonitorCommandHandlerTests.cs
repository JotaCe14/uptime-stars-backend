using FluentAssertions;
using NSubstitute;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Application.Features.CreateMonitor;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.UnitTests.Application.Features.CreateMonitor;

public class CreateMonitorCommandHandlerTests
{
    private readonly IDateTime _dateTime;
    private readonly IMonitorRepository _monitorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMonitorScheduler _scheduler;
    private readonly CreateMonitorCommandHandler _handler;

    public CreateMonitorCommandHandlerTests()
    {
        _dateTime = Substitute.For<IDateTime>();
        _monitorRepository = Substitute.For<IMonitorRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _scheduler = Substitute.For<IMonitorScheduler>();
        _handler = new CreateMonitorCommandHandler(_dateTime, _monitorRepository, _unitOfWork, _scheduler);
    }

    private static CreateMonitorCommand GetCommand()
    {
        return new CreateMonitorCommand(
            "Monitor1",
            "Description",
            Guid.NewGuid(),
            MonitorType.Ping,
            "127.0.0.1",
            1,
            1000,
            [],
            TextSearchMode.Include,
            "ok",
            [],
            "",
            0,
            0
        );
    }

    [Fact]
    public async Task Handle_CreatesMonitor_AndReturnsId()
    {
        // Arrange

        var command = GetCommand();

        _dateTime
            .UtcNow
            .Returns(DateTime.UtcNow);

        _monitorRepository
            .AddAsync(Arg.Any<ComponentMonitor>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Task.CompletedTask);

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        await _monitorRepository.Received(1).AddAsync(Arg.Is<ComponentMonitor>(monitor => 
            monitor.Name == command.Name &&
            monitor.Description == command.Description &&
            monitor.GroupId == command.GroupId &&
            monitor.Type == command.Type &&
            monitor.AlertEmails.SequenceEqual(command.AlertEmails) &&
            monitor.IntervalInMinutes == command.IntervalInMinutes &&
            monitor.TiemoutInMilliseconds == command.TiemoutInMilliseconds &&
            monitor.RequestHeaders.SequenceEqual(command.RequestHeaders) &&
            monitor.SearchMode == command.SearchMode &&
            monitor.ExpectedText == command.ExpectedText &&
            monitor.AlertDelayMinutes == command.AlertDelayMinutes &&
            monitor.AlertMessage == command.AlertMessage &&
            monitor.AlertResendCycles == command.AlertResendCycles
        ), Arg.Any<CancellationToken>());

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SchedulesMonitor()
    {
        // Arrange

        var command = GetCommand();

        _dateTime
            .UtcNow
            .Returns(DateTime.UtcNow);

        _monitorRepository
            .AddAsync(Arg.Any<ComponentMonitor>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Task.CompletedTask);

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        await _scheduler.Received(1).ScheduleAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }
}
