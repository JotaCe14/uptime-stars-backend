using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Uptime.Stars.Api.Endpoints.Abstractions;
using Uptime.Stars.Api.Extensions;
using Uptime.Stars.Api.Filters;
using Uptime.Stars.Application.Core.Abstractions.Pagination;
using Uptime.Stars.Application.Features.CreateGroup;
using Uptime.Stars.Application.Features.GetGroup;
using Uptime.Stars.Application.Features.GetGroups;
using Uptime.Stars.Application.Features.RemoveGroup;
using Uptime.Stars.Application.Features.UpdateGroup;
using Uptime.Stars.Contracts.Groups;

namespace Uptime.Stars.Api.Endpoints;

public class GroupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app
            .MapPost("group", CreateGroup)
            .WithName("Create Group")
            .WithSummary("Creates a new group")
            .WithTags("creation")
            .AddEndpointFilter<ValidationFilter<GroupRequest>>()
            .Produces<Guid>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        app
            .MapPatch("group/{groupId:guid}", UpdateGroup)
            .WithName("Update Group")
            .WithSummary("Updates a group")
            .WithTags("update")
            .AddEndpointFilter<ValidationFilter<GroupRequest>>()
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        app
            .MapGet("group/{groupId:guid}", GetGroup)
            .WithName("Get Group")
            .WithSummary("Gets a group")
            .WithTags("get")
            .Produces<GroupResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        app
            .MapGet("group", GetGroups)
            .WithName("Get Groups")
            .WithSummary("Gets groups")
            .WithTags("get")
            .Produces<PagedList<GroupResponse>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        app
            .MapDelete("group/{groupId:guid}", RemoveGroup)
            .WithName("Remove Group")
            .WithSummary("Deletes a group")
            .WithTags("remove")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ProblemDetails>(StatusCodes.Status424FailedDependency)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    public async Task<IResult> CreateGroup(
        GroupRequest request,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(request.Adapt<CreateGroupCommand>(), cancellationToken);
        if (result.IsFailure) return Results.Problem(result.Error.ToProblemDetails());
        return Results.Ok(result.Value);
    }

    public async Task<IResult> UpdateGroup(
        GroupRequest request,
        Guid groupId,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(request.Adapt<UpdateGroupCommand>() with { Id = groupId }, cancellationToken);
        if (result.IsFailure) return Results.Problem(result.Error.ToProblemDetails());
        return Results.Ok();
    }

    public async Task<IResult> GetGroup(
        Guid groupId,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetGroupQuery(groupId), cancellationToken);
        if (result.IsFailure) return Results.Problem(result.Error.ToProblemDetails());
        return Results.Ok(result.Value);
    }

    public async Task<IResult> GetGroups(
        ISender sender,
        int pageSize = 10,
        int pageNumber = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetGroupsQuery(pageSize, pageNumber), cancellationToken);
        return Results.Ok(new PagedList<GroupResponse>(
           result,
           result.PageNumber,
           result.PageSize,
           result.TotalItemCount,
           result.PageCount,
           result.HasNextPage,
           result.HasPreviousPage
       ));
    }

    public async Task<IResult> RemoveGroup(
        Guid groupId,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new RemoveGroupCommand(groupId), cancellationToken);
        if (result.IsFailure) return Results.Problem(result.Error.ToProblemDetails());
        return Results.Ok();
    }
}