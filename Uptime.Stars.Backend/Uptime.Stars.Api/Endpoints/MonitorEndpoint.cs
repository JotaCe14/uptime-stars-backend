using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Uptime.Stars.Api.Endpoints.Abstractions;
using Uptime.Stars.Api.Extensions;
using Uptime.Stars.Api.Filters;
using Uptime.Stars.Application.Core.Abstractions.Pagination;
using Uptime.Stars.Application.Features.CreateMonitor;
using Uptime.Stars.Application.Features.DisableMonitor;
using Uptime.Stars.Application.Features.EnableMonitor;
using Uptime.Stars.Application.Features.GetMonitor;
using Uptime.Stars.Application.Features.GetMonitors;
using Uptime.Stars.Application.Features.RemoveMonitor;
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
            .MapGet("monitor/{monitorId:guid}", GetMonitor)
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

        app
            .MapGet("monitor", GetMonitors)
            .WithName("Get Monitors")
            .WithSummary("Gets monitors")
            .WithTags("get")
            .Produces<PagedList<MonitorResponse>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        app
            .MapPost("monitor/disable/{monitorId:guid}", DisableMonitor)
            .WithName("Disable Monitor")
            .WithSummary("Disables a monitor")
            .WithTags("disable")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        app
            .MapPost("monitor/enable/{monitorId:guid}", EnableMonitor)
            .WithName("Enable Monitor")
            .WithSummary("Enables a monitor")
            .WithTags("enable")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        app
            .MapDelete("monitor/{monitorId:guid}", RemoveMonitor)
            .WithName("Remove Monitor")
            .WithSummary("Deletes a monitor")
            .WithTags("remove")
            .Produces(StatusCodes.Status200OK)
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

    public async Task<IResult> GetMonitors(
        ISender sender,
        int pageSize = 10,
        int pageNumber = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetMonitorsQuery(pageSize, pageNumber), cancellationToken);
        return Results.Ok(new PagedList<MonitorResponse>(
           result,
           result.PageNumber,
           result.PageSize,
           result.TotalItemCount,
           result.PageCount,
           result.HasNextPage,
           result.HasPreviousPage
       ));
    }

    public async Task<IResult> DisableMonitor(
        Guid monitorId,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DisableMonitorCommand(monitorId), cancellationToken);
        if (result.IsFailure) return Results.Problem(result.Error.ToProblemDetails());
        return Results.Ok();
    }

    public async Task<IResult> EnableMonitor(
        Guid monitorId,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new EnableMonitorCommand(monitorId), cancellationToken);
        if (result.IsFailure) return Results.Problem(result.Error.ToProblemDetails());
        return Results.Ok();
    }

    public async Task<IResult> RemoveMonitor(
        Guid monitorId,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new RemoveMonitorCommand(monitorId), cancellationToken);
        if (result.IsFailure) return Results.Problem(result.Error.ToProblemDetails());
        return Results.Ok();
    }
}