using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Application.Features.UpdateMonitor;
internal sealed class UpdateMonitorCommandHandler(
    IMonitorRepository monitorRepository, 
    IMonitorScheduler monitorScheduler,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateMonitorCommand, Result>
{
    public async Task<Result> Handle(UpdateMonitorCommand request, CancellationToken cancellationToken)
    {
        var monitor = await monitorRepository.GetByIdAsync(request.Id, cancellationToken);

        if (monitor is null)
        {
            return Result.Failure(Error.Failure("DisableMonitor.Handle", "Monitor not found"));
        }

        var previousInterval = monitor.IntervalInMinutes;

        monitor.Update(
            name: request.Name,
            description: request.Description,
            type: request.Type,
            target: request.Target,
            requestHeaders: request.RequestHeaders,
            alertEmails: request.AlertEmails,
            groupId: request.GroupId,
            intervalInMinutes: request.IntervalInMinutes,
            timeoutInMilliseconds: request.TiemoutInMilliseconds,
            searchMode: request.SearchMode,
            expectedText: request.ExpectedText,
            alertMessage: request.AlertMessage,
            alertDelayMinutes: request.AlertDelayMinutes,
            alertResendCycles: request.AlertResendCycles
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (request.IntervalInMinutes != previousInterval)
        {
            await monitorScheduler.ScheduleAsync(monitor.Id, request.IntervalInMinutes, cancellationToken);
        }

        return Result.Success();
    }
}