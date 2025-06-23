using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Uptime.Stars.Api.Endpoints;
using Uptime.Stars.Api.Extensions;
using Uptime.Stars.Application.Features.CreateMonitor;
using Uptime.Stars.Application.Features.DisableMonitor;
using Uptime.Stars.Application.Features.EnableMonitor;
using Uptime.Stars.Application.Features.GenerateReport;
using Uptime.Stars.Application.Features.GetMonitor;
using Uptime.Stars.Application.Features.RemoveMonitor;
using Uptime.Stars.Application.Features.UpdateMonitor;
using Uptime.Stars.Contracts.Events;
using Uptime.Stars.Contracts.Monitors;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.UnitTests.Endpoints;
public class MonitorEndpointTests
{
    private readonly ISender _sender;
    private readonly MonitorEndpoint _endpoint;

    public MonitorEndpointTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new MonitorEndpoint();
    }

    [Fact]
    public async Task CreateMonitor_ReturnsOk_WhenSuccess()
    {
        // Arrange
        
        var request = new MonitorRequest();

        var monitorId = Guid.NewGuid();

        _sender.Send(Arg.Any<CreateMonitorCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Result.Success(monitorId));

        // Act
        
        var result = await _endpoint.CreateMonitor(request, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<Ok<Guid>>();
        result.As<Ok<Guid>>().Value.Should().Be(monitorId);
    }

    [Fact]
    public async Task CreateMonitor_ReturnsProblem_WhenFailure()
    {
        // Arrange

        var request = new MonitorRequest();

        var failureResult = Result.Failure<Guid>(Error.Failure("CreateMonitor.Handle", "General Error"));

        _sender.Send(Arg.Any<CreateMonitorCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act

        var result = await _endpoint.CreateMonitor(request, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<ProblemHttpResult>();
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }

    [Fact]
    public async Task UpdateMonitor_ReturnsOk_WhenSuccess()
    {
        // Arrange

        var request = new MonitorRequest();
        
        var monitorId = Guid.NewGuid();

        _sender.Send(Arg.Any<UpdateMonitorCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Result.Success());

        // Act
        
        var result = await _endpoint.UpdateMonitor(request, monitorId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<Ok>();
    }

    [Fact]
    public async Task UpdateMonitor_ReturnsProblem_WhenFailure()
    {
        // Arrange
        
        var request = new MonitorRequest();
        
        var monitorId = Guid.NewGuid();
        
        var failureResult = Result.Failure(Error.Failure("UpdateMonitor.Handle", "General Error"));

        _sender.Send(Arg.Any<UpdateMonitorCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act
        
        var result = await _endpoint.UpdateMonitor(request, monitorId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<ProblemHttpResult>();
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }

    [Fact]
    public async Task GetMonitor_ReturnsOk_WhenSuccess()
    {
        // Arrange

        var monitorId = Guid.NewGuid();
        
        var monitorResponse = new MonitorResponse();
        
        var successResult = Result.Success(monitorResponse);

        _sender.Send(Arg.Any<GetMonitorQuery>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(successResult);

        // Act
        
        var result = await _endpoint.GetMonitor(monitorId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<Ok<MonitorResponse>>();
        result.As<Ok<MonitorResponse>>().Value.Should().Be(monitorResponse);
    }

    [Fact]
    public async Task GetMonitor_ReturnsProblem_WhenFailure()
    {
        // Arrange
        
        var monitorId = Guid.NewGuid();
        
        var failureResult = Result.Failure<MonitorResponse>(Error.Failure("GetMonitor.Handle", "General Error"));

        _sender.Send(Arg.Any<GetMonitorQuery>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act
        
        var result = await _endpoint.GetMonitor(monitorId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<ProblemHttpResult>();
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }

    [Fact]
    public async Task DisableMonitor_ReturnsOk_WhenSuccess()
    {
        // Arrange

        var monitorId = Guid.NewGuid();
        
        _sender.Send(Arg.Any<DisableMonitorCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Result.Success());

        // Act
        
        var result = await _endpoint.DisableMonitor(monitorId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<Ok>();
    }

    [Fact]
    public async Task DisableMonitor_ReturnsProblem_WhenFailure()
    {
        // Arrange
        
        var monitorId = Guid.NewGuid();
        
        var failureResult = Result.Failure(Error.Failure("DisableMonitor.Handle", "General Error"));

        _sender.Send(Arg.Any<DisableMonitorCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act
        
        var result = await _endpoint.DisableMonitor(monitorId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<ProblemHttpResult>();
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }

    [Fact]
    public async Task EnableMonitor_ReturnsOk_WhenSuccess()
    {
        // Arrange

        var monitorId = Guid.NewGuid();
        
        _sender.Send(Arg.Any<EnableMonitorCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Result.Success());

        // Act
        
        var result = await _endpoint.EnableMonitor(monitorId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<Ok>();
    }

    [Fact]
    public async Task EnableMonitor_ReturnsProblem_WhenFailure()
    {
        // Arrange
        
        var monitorId = Guid.NewGuid();
        
        var failureResult = Result.Failure(Error.Failure("EnableMonitor.Handle", "General Error"));

        _sender.Send(Arg.Any<EnableMonitorCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act
        
        var result = await _endpoint.EnableMonitor(monitorId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<ProblemHttpResult>();
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }

    [Fact]
    public async Task RemoveMonitor_ReturnsOk_WhenSuccess()
    {
        // Arrange
        
        var monitorId = Guid.NewGuid();
        
        _sender.Send(Arg.Any<RemoveMonitorCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Result.Success());

        // Act
        
        var result = await _endpoint.RemoveMonitor(monitorId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<Ok>();
    }

    [Fact]
    public async Task RemoveMonitor_ReturnsProblem_WhenFailure()
    {
        // Arrange
        
        var monitorId = Guid.NewGuid();
        
        var failureResult = Result.Failure(Error.Failure("RemoveMonitor.Handle", "General Error"));

        _sender.Send(Arg.Any<RemoveMonitorCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act
        
        var result = await _endpoint.RemoveMonitor(monitorId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<ProblemHttpResult>();
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }

    [Fact]
    public async Task GenerateMonitorReport_ReturnsFile_WhenSuccess()
    {
        // Arrange
        
        var dateFrom = "01/01/2025";
        
        var dateTo = "31/01/2025";
        
        var fileContent = new byte[] { 1, 2, 3 };
        
        var successResult = Result.Success(fileContent);

        _sender.Send(Arg.Any<GenerateMonitorReportCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(successResult);

        // Act
        
        var result = await _endpoint.GenerateMonitorReport(dateFrom, dateTo, _sender, CancellationToken.None);

        // Assert

        result.Should().BeOfType<FileContentHttpResult>();
        result.As<FileContentHttpResult>().ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        result.As<FileContentHttpResult>().FileDownloadName.Should().Be($"monitor-{dateFrom}-{dateTo}.xlsx");
    }

    [Fact]
    public async Task GenerateMonitorReport_ReturnsProblem_WhenFailure()
    {
        // Arrange

        var dateFrom = "01/01/2025";

        var dateTo = "31/01/2025";

        var failureResult = Result.Failure<byte[]>(Error.Failure("GenerateMonitorReport.Handle", "General Error"));

        _sender.Send(Arg.Any<GenerateMonitorReportCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act
        
        var result = await _endpoint.GenerateMonitorReport(dateFrom, dateTo, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<ProblemHttpResult>();
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }
}