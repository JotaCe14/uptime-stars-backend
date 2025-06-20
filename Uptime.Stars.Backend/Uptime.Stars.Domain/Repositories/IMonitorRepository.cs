using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Domain.Repositories;
public interface IMonitorRepository
{
    Task AddAsync(ComponentMonitor monitor, CancellationToken cancellationToken = default);
    Task<ComponentMonitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}