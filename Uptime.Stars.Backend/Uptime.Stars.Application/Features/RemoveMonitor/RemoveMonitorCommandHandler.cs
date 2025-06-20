using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Application.Features.RemoveMonitor;
internal sealed class RemoveMonitorCommandHandler(
    IMonitorRepository monitorRepository, 
    IMonitorScheduler scheduler) : ICommandHandler<RemoveMonitorCommand, Result>
{
    public async Task<Result> Handle(RemoveMonitorCommand request, CancellationToken cancellationToken)
    {
        var monitor = await monitorRepository.GetByIdAsync(request.MonitorId, cancellationToken);

        if (monitor is null)
        {
            return Result.Failure(Error.Failure("EnableMonitor.Handle", "Monitor not found"));
        }

        await scheduler.RemoveAsync(monitor.Id, cancellationToken);

        await monitorRepository.DeleteAsync(monitor.Id, cancellationToken);

        return Result.Success();
    }
}