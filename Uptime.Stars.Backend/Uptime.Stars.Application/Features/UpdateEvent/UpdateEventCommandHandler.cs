using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Application.Features.UpdateEvent;
internal sealed class UpdateEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork): ICommandHandler<UpdateEventCommand, Result>
{
    public async Task<Result> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(request.Id, cancellationToken);

        if (@event is null)
        {
            return Result.Failure(Error.Failure("UpdateEvent.Handle", "Event not found"));
        }

        @event.Update(
            request.Category,
            request.MaintenanceType,
            request.FalsePositive,
            request.Note,
            request.TicketId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}