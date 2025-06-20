using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Domain.Repositories;
public interface IEventRepository
{
    Task AddAsync(Event @event, CancellationToken cancellationToken = default);
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Event>> GetLastByMonitorIdAsync(Guid monitorId, int limit = 50, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Event>> GetLastByMonitorIdSinceAsync(Guid monitorId, DateTime sinceDateTime, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Event>> GetLastImportantByMonitorIdAsync(Guid monitorId, int limit = 20, CancellationToken cancellationToken = default);
    Task<bool> IsFirstByMonitorIdAsync(Guid monitorId, CancellationToken cancellationToken = default);
}