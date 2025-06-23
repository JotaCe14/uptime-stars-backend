using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Uptime.Stars.Application.Features.RemoveGroup;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.UnitTests.Application.Features.RemoveGroup;

public class RemoveGroupCommandHandlerTests
{
    private readonly IGroupRepository _groupRepository;
    private readonly RemoveGroupCommandHandler _handler;

    public RemoveGroupCommandHandlerTests()
    {
        _groupRepository = Substitute.For<IGroupRepository>();
        _handler = new RemoveGroupCommandHandler(_groupRepository);
    }

    [Fact]
    public async Task Handle_GroupExists_RemovesGroupAndReturnsSuccess()
    {
        // Arrange

        var groupId = Guid.NewGuid();
        
        var group = new Group { Name = "Test", Description = "Desc" };
        
        var command = new RemoveGroupCommand(groupId);

        _groupRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(group);

        _groupRepository
            .DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Task.CompletedTask);

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        result.IsSuccess.Should().BeTrue();
        await _groupRepository.Received(1).DeleteAsync(group.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_GroupDoesNotExist_ReturnsFailure()
    {
        // Arrange

        var groupId = Guid.NewGuid();
        
        var command = new RemoveGroupCommand(groupId);

        _groupRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsNullForAnyArgs();

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("RemoveGroup.Handle");
        result.Error.Description.Should().Be("Group not found");
        
        await _groupRepository.DidNotReceive().DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
