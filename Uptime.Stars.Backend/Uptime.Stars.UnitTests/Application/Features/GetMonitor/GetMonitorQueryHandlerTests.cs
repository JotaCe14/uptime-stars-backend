using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Uptime.Stars.Application.Features.GetMonitor;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.UnitTests.Application.Features.GetMonitor;

public class GetMonitorQueryHandlerTests
{
    private readonly IMonitorRepository _monitorRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IEventService _eventService;
    private readonly GetMonitorQueryHandler _handler;

    public GetMonitorQueryHandlerTests()
    {
        _monitorRepository = Substitute.For<IMonitorRepository>();
        _eventRepository = Substitute.For<IEventRepository>();
        _eventService = Substitute.For<IEventService>();
        _handler = new GetMonitorQueryHandler(_monitorRepository, _eventRepository, _eventService);
    }

    [Fact]
    public async Task Handle_MonitorNotFound_ReturnsFailure()
    {
        // Arrange

        var monitorId = Guid.NewGuid();
        
        var query = new GetMonitorQuery(monitorId, 5);

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsNullForAnyArgs();

        // Act

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("GetMonitor.Handle");
        result.Error.Description.Should().Be("Monitor not found");
    }

    [Fact]
    public async Task Handle_MonitorFound_ReturnsMonitorResponse()
    {
        // Arrange
        
        var monitor = ComponentMonitor.Create(
            "Monitor 1",
            "Desc",
            MonitorType.Https, 
            "http://test.pe", 
            DateTime.UtcNow, 
            [], 
            []);

        var events = new List<Event>
        {
            Event.Create(monitor.Id, DateTime.UtcNow, 1)
        };

        var query = new GetMonitorQuery(monitor.Id, 5);

        _monitorRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(monitor);

        _eventRepository
            .GetLastImportantByMonitorIdAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(events);

        _eventService
            .GetUptimePercentageLastSince(monitor.Id, TimeSpan.FromHours(24), Arg.Any<CancellationToken>())
            .Returns(99.5M);

        _eventService
            .GetUptimePercentageLastSince(monitor.Id, TimeSpan.FromDays(30), Arg.Any<CancellationToken>())
            .Returns(98.1M);

        // Act

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert

        result.IsSuccess.Should().BeTrue();

        result.Value.Id.Should();
        result.Value.Name.Should().Be("Monitor 1");
        result.Value.Description.Should().Be("Desc");
        result.Value.IsUp.Should().BeNull();
        result.Value.IntervalInMinutes.Should().Be(1);
        result.Value.Target.Should().Be("http://test.pe");
        result.Value.IsActive.Should().BeTrue();
        result.Value.LastEvents.Should().HaveCount(1);
        result.Value.Uptime24hPercentage.Should().Be("99.5%");
        result.Value.Uptime30dPercentage.Should().Be("98.1%");
    }
}
