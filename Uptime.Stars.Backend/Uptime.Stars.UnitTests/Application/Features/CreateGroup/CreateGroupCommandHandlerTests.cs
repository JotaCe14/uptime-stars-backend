using FluentAssertions;
using NSubstitute;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Features.CreateGroup;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.UnitTests.Application.Features.CreateGroup;

public class CreateGroupCommandHandlerTests
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateGroupCommandHandler _handler;

    public CreateGroupCommandHandlerTests()
    {
        _groupRepository = Substitute.For<IGroupRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateGroupCommandHandler(_groupRepository, _unitOfWork);
    }

    private static CreateGroupCommand GetCommand()
    {
        return new CreateGroupCommand("Test Group", "Test Description");
    }

    [Fact]
    public async Task Handle_CreatesGroup_AndReturnsId()
    {
        // Arrange
        
        var command = GetCommand();
        
        _groupRepository
            .AddAsync(Arg.Any<Group>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Task.CompletedTask);

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        await _groupRepository.Received(1).AddAsync(Arg.Is<Group>(group => group.Name == command.Name && group.Description == command.Description), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
