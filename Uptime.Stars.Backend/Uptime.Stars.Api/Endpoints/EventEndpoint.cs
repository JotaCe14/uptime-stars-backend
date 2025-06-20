using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Uptime.Stars.Api.Endpoints.Abstractions;
using Uptime.Stars.Api.Extensions;
using Uptime.Stars.Api.Filters;
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
}