using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Uptime.Stars.Api.Endpoints;
using Uptime.Stars.Api.Extensions;
using Uptime.Stars.Application.Features.CreateGroup;
using Uptime.Stars.Application.Features.GetGroup;
using Uptime.Stars.Application.Features.RemoveGroup;
using Uptime.Stars.Application.Features.UpdateGroup;
using Uptime.Stars.Contracts.Groups;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.UnitTests.Endpoints;

public class GroupEndpointTests
{
    private readonly ISender _sender;
    private readonly GroupEndpoint _endpoint;

    public GroupEndpointTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GroupEndpoint();
    }

    public static GroupResponse GetGroup()
    {
        return new GroupResponse(Guid.NewGuid(), "");
    }

    [Fact]
    public async Task CreateGroup_ReturnsOk_WhenSuccess()
    {
        // Arrange

        var request = new GroupRequest();
        
        var groupId = Guid.NewGuid();
        
        _sender.Send(Arg.Any<CreateGroupCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Result.Success(groupId));

        // Act

        var result = await _endpoint.CreateGroup(request, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<Ok<Guid>>();
        result.As<Ok<Guid>>().Value.Should().Be(groupId);
    }

    [Fact]
    public async Task CreateGroup_ReturnsProblem_WhenFailure()
    {
        // Arrange

        var request = new GroupRequest();
        
        var failureResult = Result.Failure<Guid>(Error.Failure("CreateGroup.Handle", "General Error"));
        
        _sender.Send(Arg.Any<CreateGroupCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act

        var result = await _endpoint.CreateGroup(request, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<ProblemHttpResult>();
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }

    [Fact]
    public async Task UpdateGroup_ReturnsOk_WhenSuccess()
    {
        // Arrange
        
        var request = new GroupRequest();
        
        var groupId = Guid.NewGuid();
        
        _sender.Send(Arg.Any<UpdateGroupCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Result.Success());

        // Act

        var result = await _endpoint.UpdateGroup(request, groupId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<Ok>();
    }

    [Fact]
    public async Task UpdateGroup_ReturnsProblem_WhenFailure()
    {
        // Arrange
        
        var request = new GroupRequest();
        
        var groupId = Guid.NewGuid();
        
        var failureResult = Result.Failure(Error.Failure("UpdateGroup.Handle", "General Error"));
        
        _sender.Send(Arg.Any<UpdateGroupCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act

        var result = await _endpoint.UpdateGroup(request, groupId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<ProblemHttpResult>();
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }

    [Fact]
    public async Task GetGroup_ReturnsOk_WhenSuccess()
    {
        // Arrange

        var groupId = Guid.NewGuid();
        var groupResponse = GetGroup();
        
        _sender.Send(Arg.Any<GetGroupQuery>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Result.Success(groupResponse));

        // Act

        var result = await _endpoint.GetGroup(groupId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<Ok<GroupResponse>>();
        result.As<Ok<GroupResponse>>().Value.Should().Be(groupResponse);
    }

    [Fact]
    public async Task GetGroup_ReturnsProblem_WhenFailure()
    {
        // Arrange

        var groupId = Guid.NewGuid();
        
        var failureResult = Result.Failure<GroupResponse>(Error.Failure("GetGroup.Handle", "General Error"));
        
        _sender.Send(Arg.Any<GetGroupQuery>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act

        var result = await _endpoint.GetGroup(groupId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<ProblemHttpResult>();
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }

    [Fact]
    public async Task RemoveGroup_ReturnsOk_WhenSuccess()
    {
        // Arrange

        var groupId = Guid.NewGuid();
        
        _sender.Send(Arg.Any<RemoveGroupCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Result.Success());

        // Act

        var result = await _endpoint.RemoveGroup(groupId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<Ok>();
    }

    [Fact]
    public async Task RemoveGroup_ReturnsProblem_WhenFailure()
    {
        // Arrange
        
        var groupId = Guid.NewGuid();
        
        var failureResult = Result.Failure(Error.Failure("RemoveGroup.Handle", "General Error"));
        
        _sender.Send(Arg.Any<RemoveGroupCommand>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(failureResult);

        // Act

        var result = await _endpoint.RemoveGroup(groupId, _sender, CancellationToken.None);

        // Assert
        
        result.Should().BeOfType<ProblemHttpResult>();
        result.As<ProblemHttpResult>().ProblemDetails.Should().BeEquivalentTo(failureResult.Error.ToProblemDetails());
    }
}
