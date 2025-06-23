using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Features.UpdateGroup;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.UnitTests.Application.Features.UpdateGroup;

public class UpdateGroupCommandHandlerTests
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateGroupCommandHandler _handler;

    public UpdateGroupCommandHandlerTests()
    {
        _groupRepository = Substitute.For<IGroupRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new UpdateGroupCommandHandler(_groupRepository, _unitOfWork);
    }

    private static Group GetGroup()
    {
        return new Group
        {
            Name = "Old Name",
            Description = "Old Description"
        };
    }

    [Fact]
    public async Task Handle_GroupNotFound_ReturnsFailure()
    {
        // Arrange

        var command = new UpdateGroupCommand(Guid.NewGuid(), "New Name", "New Description");
        
        _groupRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsNullForAnyArgs();

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("UpdateGroup.Handle");
        result.Error.Description.Should().Be("Group not found");
    }

    [Fact]
    public async Task Handle_UpdatesGroup_AndReturnsSuccess()
    {
        // Arrange
        
        var group = GetGroup();
        
        var command = new UpdateGroupCommand(group.Id, "Updated Name", "Updated Description");

        _groupRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(group);

        _unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        
        result.IsSuccess.Should().BeTrue();
        group.Name.Should().Be("Updated Name");
        group.Description.Should().Be("Updated Description");
    }
}
