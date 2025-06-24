using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Uptime.Stars.Api.Endpoints.Abstractions;
using Uptime.Stars.Api.Extensions;
using Uptime.Stars.Api.Filters;
using Uptime.Stars.Application.Core.Abstractions.Pagination;
using Uptime.Stars.Application.Features.GenerateEventsReport;
using Uptime.Stars.Application.Features.GetEvents;
using Uptime.Stars.Application.Features.UpdateEvent;
using Uptime.Stars.Contracts.Events;

namespace Uptime.Stars.Api.Endpoints;

public class EventEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app
            .MapPatch("event/{eventId:guid}", UpdateEvent)
            .WithName("Update Event")
            .WithSummary("Updates an event")
            .WithTags("update")
            .AddEndpointFilter<ValidationFilter<EventRequest>>()
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        app
            .MapGet("event/{monitorId:guid?}", GetImportantEvents)
            .WithName("Get events")
            .WithSummary("Gets the events from a monitor or in total")
            .WithTags("get")
            .Produces<PagedList<EventResponse>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        app
            .MapGet("event/export/{monitorId:guid}", GenerateEventsReport)
            .WithName("Generate Events Report")
            .WithSummary("Genereates a report from events")
            .WithTags("export")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    public async Task<IResult> UpdateEvent(
        EventRequest request,
        Guid eventId,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(request.Adapt<UpdateEventCommand>() with { Id = eventId }, cancellationToken);
        if (result.IsFailure) return Results.Problem(result.Error.ToProblemDetails());
        return Results.Ok();
    }
    public async Task<IResult> GetImportantEvents(
        ISender sender,
        int pageSize = 10,
        int pageNumber = 1,
        Guid? monitorId = default,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetImportantEventsQuery(pageSize, pageNumber, monitorId), cancellationToken);
        return Results.Ok(new PagedList<EventResponse>(
           result,
           result.PageNumber,
           result.PageSize,
           result.TotalItemCount,
           result.PageCount,
           result.HasNextPage,
           result.HasPreviousPage
       ));
    }

    public async Task<IResult> GenerateEventsReport(
        Guid monitorId,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GenerateEventsReportCommand(monitorId), cancellationToken);
        if (result.IsFailure) return Results.Problem(result.Error.ToProblemDetails());
        return Results.File(result.Value, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"monitor-events-{monitorId}.xlsx");
    }
}