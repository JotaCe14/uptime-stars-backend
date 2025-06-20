using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;
using Uptime.Stars.Infrastructure.Strategies;

namespace Uptime.Stars.Infrastructure.Jobs;
internal sealed class MonitorJob(ICheckStrategyFactory checkStrategyFactory, IDateTime dateTime, IMonitorRepository monitorRepository, IEventRepository eventRepository, IUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(Guid monitorId, CancellationToken cancellationToken = default)
    {
        var monitor = await monitorRepository.GetByIdAsync(monitorId, cancellationToken);

        if (monitor is null || !monitor.IsActive)
        {
            return;
        }

        var checkStrategy = checkStrategyFactory.GetStrategy(monitor.Type) ?? throw new InvalidOperationException($"No check strategy found for monitor type: {monitor.Type}");

        var checkResult = await checkStrategy.CheckAsync(monitor, cancellationToken);

        var @event = Event.Create(monitor.Id, dateTime.UtcNow, checkResult.IsUp, checkResult.Message, checkResult.LatencyMilliseconds);

        await eventRepository.AddAsync(@event, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return;
    }
}