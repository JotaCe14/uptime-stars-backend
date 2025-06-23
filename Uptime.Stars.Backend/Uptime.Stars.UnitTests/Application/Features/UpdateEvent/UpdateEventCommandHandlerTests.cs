using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Features.UpdateEvent;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.UnitTests.Application.Features.UpdateEvent;

public class UpdateEventCommandHandlerTests
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateEventCommandHandler _handler;

    public UpdateEventCommandHandlerTests()
    {
        _eventRepository = Substitute.For<IEventRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new UpdateEventCommandHandler(_eventRepository, _unitOfWork);
    }

    private static Event GetEvent()
    {
        return Event.Create(Guid.NewGuid(), DateTime.UtcNow, 1);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        // Arrange

        var command = new UpdateEventCommand(
            Guid.NewGuid(),
            Category.Internal,
            MaintenanceType.Planned,
            true,
            "New note",
            "T-2"
        );

        _eventRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsNullForAnyArgs();

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("UpdateEvent.Handle");
        result.Error.Description.Should().Be("Event not found");
    }

    [Fact]
    public async Task Handle_EventFound_UpdatesEventAndReturnsSuccess()
    {
        // Arrange

        var @event = GetEvent();
        
        var command = new UpdateEventCommand(
            @event.Id,
            Category.Internal,
            MaintenanceType.Planned,
            true,
            "Updated note",
            "T-2"
        );

        _eventRepository
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(@event);

        _unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act

        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        @event.Category.Should().Be(Category.Internal);
        @event.MaintenanceType.Should().Be(MaintenanceType.Planned);
        @event.FalsePositive.Should().BeTrue();
        @event.Note.Should().Be("Updated note");
        @event.TicketId.Should().Be("T-2");
    }
}
