using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Application.Features.DisableMonitor;
internal sealed class DisableMonitorCommandHandler(
    IMonitorRepository monitorRepository, 
    IUnitOfWork unitOfWork,
    IMonitorScheduler scheduler) : ICommandHandler<DisableMonitorCommand, Result>
{
    public async Task<Result> Handle(DisableMonitorCommand request, CancellationToken cancellationToken)
    {
        var monitor = await monitorRepository.GetByIdAsync(request.MonitorId, cancellationToken);

        if (monitor is null)
        {
            return Result.Failure(Error.Failure("DisableMonitor.Handle", "Monitor not found"));
        }

        if (!monitor.IsActive)
        {
            return Result.Failure(Error.Failure("DisableMonitor.Handle", "Monitor is already disabled"));
        }

        monitor.Disable();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await scheduler.RemoveAsync(monitor.Id, cancellationToken);

        return Result.Success();
    }
}