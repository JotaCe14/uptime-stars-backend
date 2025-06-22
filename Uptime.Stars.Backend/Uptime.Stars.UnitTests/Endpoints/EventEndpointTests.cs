using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Uptime.Stars.Api.Endpoints;
using Uptime.Stars.Api.Extensions;
using Uptime.Stars.Application.Features.GenerateEventsReport;
using Uptime.Stars.Application.Features.UpdateEvent;
using Uptime.Stars.Contracts.Events;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.UnitTests.Endpoints;

public class EventEndpointTests
{
    private readonly ISender _sender;
    private readonly EventEndpoint _endpoint;

    public EventEndpointTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new EventEndpoint();
    }

    [Fact]
    public async Task UpdateEvent_ReturnsOk_WhenSuccess()
    {
        // Arrange
        
        var eventId = Guid.NewGuid();
        
        var request = new EventRequest();

        _sender.Send(Arg.Any<UpdateEventCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Result.Success());

        // Act
        
        var result = await _endpoint.UpdateEvent(request, eventId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<Ok>();
    }

    [Fact]
    public async Task UpdateEvent_ReturnsProblem_WhenFailure()
    {
        // Arrange
        
        var eventId = Guid.NewGuid();
        
        var request = new EventRequest();
        
        var failureResult = Result.Failure(Error.Failure("UpdateEvent.Handle", "General Error"));

        _sender.Send(Arg.Any<UpdateEventCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act

        var result = await _endpoint.UpdateEvent(request, eventId, _sender, CancellationToken.None);

        // Assert

        result.Should().BeOfType(typeof(ProblemHttpResult));
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }

    [Fact]
    public async Task GenerateEventsReport_ReturnsFile_WhenSuccess()
    {
        // Arrange
        
        var monitorId = Guid.NewGuid();

        var fileContent = new byte[] { 1, 2, 3 };
        
        _sender.Send(Arg.Any<GenerateEventsReportCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Result.Success(fileContent));

        // Act

        var result = await _endpoint.GenerateEventsReport(monitorId, _sender, CancellationToken.None);

        
        // Assert

        result.Should().BeOfType<FileContentHttpResult>();
        result.As<FileContentHttpResult>().ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        result.As<FileContentHttpResult>().FileDownloadName.Should().Be($"monitor-events-{monitorId}.xlsx");
    }

    [Fact]
    public async Task GenerateEventsReport_ReturnsProblem_WhenFailure()
    {
        // Arrange

        var monitorId = Guid.NewGuid();

        var failureResult = Result.Failure<byte[]>(Error.Failure("UpdateEvent.Handle", "General Error"));

        _sender.Send(Arg.Any<GenerateEventsReportCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act

        var result = await _endpoint.GenerateEventsReport(monitorId, _sender, CancellationToken.None);

        // Assert

        result.Should().BeOfType(typeof(ProblemHttpResult));
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }
}