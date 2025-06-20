using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Application.Features.EnableMonitor;
internal sealed class EnableMonitorCommandHandler(IMonitorRepository monitorRepository, IUnitOfWork unitOfWork) : ICommandHandler<EnableMonitorCommand, Result>
{
    public async Task<Result> Handle(EnableMonitorCommand request, CancellationToken cancellationToken)
    {
        var monitor = await monitorRepository.GetByIdAsync(request.MonitorId, cancellationToken);

        if (monitor is null)
        {
            return Result.Failure(Error.Failure("EnableMonitor.Handle", "Monitor not found"));
        }

        if (monitor.IsActive)
        {
            return Result.Failure(Error.Failure("EnableMonitor.Handle", "Monitor is already enabled"));
        }

        monitor.Enable();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}