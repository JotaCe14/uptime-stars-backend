using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using System.Net;
using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;
using Uptime.Stars.Infrastructure.Strategies.Implementations;

namespace Uptime.Stars.UnitTests.Infrastructure;

public class HttpsGetCheckStrategyTests
{
    private readonly HttpClient _httpClient;
    private readonly IClockTimer _timer;
    private readonly HttpsGetCheckStrategy _strategy;
    public HttpsGetCheckStrategyTests()
    {
        _httpClient = Substitute.For<HttpClient>();
        _timer = Substitute.For<IClockTimer>();
        _strategy = new HttpsGetCheckStrategy(_httpClient, _timer);
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
    public async Task CheckAsync_ReturnsUp_WhenResponseIsSuccessAndTextMatches()
    {
        // Arrange

        var monitor = GetMonitor("ok", TextSearchMode.Include);

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("This is ok!")
        };

        _httpClient.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(response);

        _timer.ElapsedMilliseconds.ReturnsForAnyArgs(123);

        // Act

        var result = await _strategy.CheckAsync(monitor);

        // Assert

        result.IsUp.Should().BeTrue();
        result.LatencyMilliseconds.Should().Be(123);
    }

    [Fact]
    public async Task CheckAsync_ReturnsDown_WhenResponseIsNull()
    {
        // Arrange

        var monitor = GetMonitor();

        _httpClient.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ReturnsNullForAnyArgs();

        _timer.ElapsedMilliseconds.ReturnsForAnyArgs(123);

        // Act
        var result = await _strategy.CheckAsync(monitor);

        // Assert
        result.IsUp.Should().BeFalse();
        result.Message.Should().Contain("No response");
    }

    [Fact]
    public async Task CheckAsync_ReturnsDown_WhenStatusCodeIsNotSuccess()
    {
        // Arrange

        var monitor = GetMonitor();

        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _httpClient.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(response);

        _timer.ElapsedMilliseconds.ReturnsForAnyArgs(123);

        // Act

        var result = await _strategy.CheckAsync(monitor);

        // Assert

        result.IsUp.Should().BeFalse();
        result.Message.Should().Contain($"Failed with status code: {(int) response.StatusCode}");
    }

    [Fact]
    public async Task CheckAsync_ReturnsDown_WhenExpectedTextNotFound()
    {
        // Arrange

        var monitor = GetMonitor("notfound", TextSearchMode.Include);

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("other content")
        };

        _httpClient.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(response);

        _timer.ElapsedMilliseconds.ReturnsForAnyArgs(123);

        // Act

        var result = await _strategy.CheckAsync(monitor);

        // Assert

        result.IsUp.Should().BeFalse();
        result.Message.Should().Contain("Expected text not found");
    }

    [Fact]
    public async Task CheckAsync_ReturnsDown_WhenUnexpectedTextFound()
    {
        // Arrange

        var monitor = GetMonitor("bad", TextSearchMode.NotInclude);

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("this is bad")
        };

        _httpClient.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(response);

        _timer.ElapsedMilliseconds.ReturnsForAnyArgs(123);

        // Act

        var result = await _strategy.CheckAsync(monitor);

        // Assert

        result.IsUp.Should().BeFalse();
        result.Message.Should().Contain("Unexpected text found");
    }

    [Fact]
    public async Task CheckAsync_ReturnsDown_WhenExceptionThrown()
    {
        // Arrange

        var monitor = GetMonitor();

        _httpClient.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ThrowsAsyncForAnyArgs(new HttpRequestException("Network error"));

        _timer.ElapsedMilliseconds.ReturnsForAnyArgs(123);

        // Act

        var result = await _strategy.CheckAsync(monitor);

        // Assert

        result.IsUp.Should().BeFalse();
        result.Message.Should().Contain("Network error");
    }

}