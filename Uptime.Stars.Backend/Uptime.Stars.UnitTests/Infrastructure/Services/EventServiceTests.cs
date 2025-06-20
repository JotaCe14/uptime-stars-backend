using FluentAssertions;
using NSubstitute;
using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;
using Uptime.Stars.Infrastructure.Services;

namespace Uptime.Stars.UnitTests.Infrastructure.Services;

public class EventServiceTests
{
    private readonly IDateTime _dateTime = Substitute.For<IDateTime>();
    private readonly IEventRepository _eventRepository = Substitute.For<IEventRepository>();
    private readonly EventService _service;

    public EventServiceTests()
    {
        _service = new EventService(_dateTime, _eventRepository);
    }

    [Fact]
    public async Task GetUptimePercentageLastSince_ReturnsNull_WhenNoEvents()
    {
        // Arrange

        _dateTime.UtcNow.Returns(DateTime.UtcNow);
        
        _eventRepository
            .GetLastByMonitorIdSinceAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs([]);

        // Act

        var result = await _service.GetUptimePercentageLastSince(Guid.NewGuid(), TimeSpan.FromHours(24), CancellationToken.None);

        // Assert
        
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUptimePercentageLastSince_ReturnsCorrectPercentage()
    {
        // Arrange

        IReadOnlyCollection <Event> events = [
            Event.Create(Guid.NewGuid(), DateTime.UtcNow),
            Event.Create(Guid.NewGuid(), DateTime.UtcNow),
            Event.Create(Guid.NewGuid(), DateTime.UtcNow, isUp: false)
        ];

        _dateTime.UtcNow.Returns(DateTime.UtcNow);

        _eventRepository
            .GetLastByMonitorIdSinceAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(events);

        // Act

        var result = await _service.GetUptimePercentageLastSince(Guid.NewGuid(), TimeSpan.FromHours(24), CancellationToken.None);

        // Assert

        result.Should().Be(66.67M);
    }

    [Fact]
    public async Task GetUptimePercentageLastSince_RoundsToTwoDecimals()
    {
        // Arrange

        IReadOnlyCollection<Event> events = [
            Event.Create(Guid.NewGuid(), DateTime.UtcNow),
            Event.Create(Guid.NewGuid(), DateTime.UtcNow),
            Event.Create(Guid.NewGuid(), DateTime.UtcNow, isUp: false),
            Event.Create(Guid.NewGuid(), DateTime.UtcNow, isUp: false),
            Event.Create(Guid.NewGuid(), DateTime.UtcNow)
        ];

        _dateTime.UtcNow.Returns(DateTime.UtcNow);

        _eventRepository
            .GetLastByMonitorIdSinceAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(events);

        // Act

        var result = await _service.GetUptimePercentageLastSince(Guid.NewGuid(), TimeSpan.FromHours(24), CancellationToken.None);

        // Assert

        result.Should().Be(60);
    }
}