using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Application.Features.CreateMonitor;
internal sealed class CreateMonitorCommandHandler(
    IDateTime dateTime, 
    IMonitorRepository monitorRepository, 
    IUnitOfWork unitOfWork,
    IMonitorScheduler scheduler) : ICommandHandler<CreateMonitorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateMonitorCommand request, CancellationToken cancellationToken)
    {
        var monitor = ComponentMonitor.Create(
            request.Name,
            request.Description,
            request.Type,
            request.Target,
            dateTime.UtcNow,
            request.RequestHeaders,
            request.AlertEmails,
            request.GroupId,
            request.IntervalInMinutes,
            request.TiemoutInMilliseconds,
            request.SearchMode,
            request.ExpectedText,
            request.AlertMessage,
            request.AlertDelayMinutes,
            request.AlertResendCycles);

        await monitorRepository.AddAsync(monitor, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await scheduler.ScheduleAsync(monitor.Id, monitor.IntervalInMinutes, cancellationToken);

        return monitor.Id;
    }
}