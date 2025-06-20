using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Uptime.Stars.Api.Endpoints.Abstractions;
using Uptime.Stars.Api.Extensions;
using Uptime.Stars.Api.Filters;
using Uptime.Stars.Application.Features.CreateMonitor;
using Uptime.Stars.Application.Features.GetMonitor;
using Uptime.Stars.Contracts.Monitor;

namespace Uptime.Stars.Api.Endpoints;

public class MonitorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app
            .MapPost("monitor", CreateMonitor)
            .WithName("Create Monitor")
            .WithSummary("Creates a new monitor")
            .WithTags("creation")
            .AddEndpointFilter<ValidationFilter<MonitorRequest>>()
            .Produces<Guid>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        app
            .MapGet("{monitorId:guid}", GetMonitor)
            .WithName("Get Monitor")
            .WithSummary("Gets a monitor")
            .WithTags("get")
            .Produces<MonitorResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    public async Task<IResult> CreateMonitor(
        MonitorRequest request,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(request.Adapt<CreateMonitorCommand>(), cancellationToken);
        if (result.IsFailure) return Results.Problem(result.Error.ToProblemDetails());
        return Results.Ok(result.Value);
    }

    public async Task<IResult> GetMonitor(
        Guid monitorId,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetMonitorQuery(monitorId), cancellationToken);
        if (result.IsFailure) return Results.Problem(result.Error.ToProblemDetails());
        return Results.Ok(result.Value);
    }
}