using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Uptime.Stars.Application.Features.GetGroup;
using Uptime.Stars.Contracts.Groups;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.UnitTests.Application.Features.GetGroup;

public class GetGroupQueryHandlerTests
{
    private readonly IGroupRepository _groupRepository;
    private readonly GetGroupQueryHandler _handler;

    public GetGroupQueryHandlerTests()
    {
        _groupRepository = Substitute.For<IGroupRepository>();
        _handler = new GetGroupQueryHandler(_groupRepository);
    }

    [Fact]
    public async Task Handle_GroupExists_ReturnsGroupResponse()
    {
        // Arrange
        
        var groupId = Guid.NewGuid();
        
        var group = new Group { Name = "Test Group", Description = "Test Description" };
        
        var query = new GetGroupQuery(groupId);

        _groupRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(group);

        // Act

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new GroupResponse(group.Id, "Test Group", "Test Description"));
    }

    [Fact]
    public async Task Handle_GroupDoesNotExist_ReturnsFailure()
    {
        // Arrange

        var groupId = Guid.NewGuid();
        
        var query = new GetGroupQuery(groupId);

        _groupRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsNullForAnyArgs();

        // Act

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("GetGroup.Handle");
        result.Error.Description.Should().Be("Group not found");
    }
}
