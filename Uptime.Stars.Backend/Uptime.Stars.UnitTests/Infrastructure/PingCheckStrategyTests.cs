using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using System.Net.NetworkInformation;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;
using Uptime.Stars.Infrastructure.Strategies.Implementations;

namespace Uptime.Stars.UnitTests.Infrastructure;

public class PingCheckStrategyTests
{
    private readonly IPingWrapper _pingWrapper;
    private readonly PingCheckStrategy _strategy;

    public PingCheckStrategyTests()
    {
        _pingWrapper = Substitute.For<IPingWrapper>();
        _strategy = new PingCheckStrategy(_pingWrapper);
    }

    private static ComponentMonitor GetMonitor(int timeout = 1000)
    {
        return ComponentMonitor.Create(
            "",
            "",
            MonitorType.Ping,
            "127.0.0.1",
            DateTime.UtcNow,
            [],
            [],
            timeoutInMilliseconds: timeout);
    }

    [Fact]
    public async Task CheckAsync_ReturnsUp_WhenReplyIsValid()
    {
        // Arrange

        var monitor = GetMonitor();

        var reply = new PingReplyWrapper(42);

        _pingWrapper.SendPingAsync(Arg.Any<string>(), Arg.Any<int>())
            .ReturnsForAnyArgs(reply);

        // Act

        var result = await _strategy.CheckAsync(monitor);

        // Assert
        
        result.IsUp.Should().BeTrue();
        result.LatencyMilliseconds.Should().Be(42);
    }

    [Fact]
    public async Task CheckAsync_ReturnsDown_WhenReplyIsNull()
    {
        // Arrange

        var monitor = GetMonitor();
        
        _pingWrapper.SendPingAsync(Arg.Any<string>(), Arg.Any<int>())
            .ReturnsNullForAnyArgs();

        // Act

        var result = await _strategy.CheckAsync(monitor);

        // Assert

        result.IsUp.Should().BeFalse();
        result.Message.Should().Contain("No response");
    }

    [Fact]
    public async Task CheckAsync_ReturnsDown_WhenExceptionThrown()
    {
        // Arrange

        var monitor = GetMonitor();
        
        _pingWrapper.SendPingAsync(Arg.Any<string>(), Arg.Any<int>())
            .ThrowsAsyncForAnyArgs(new Exception("Ping failed"));

        // Act

        var result = await _strategy.CheckAsync(monitor);

        // Assert
        
        result.IsUp.Should().BeFalse();
        result.Message.Should().Contain("Ping failed");
    }
}