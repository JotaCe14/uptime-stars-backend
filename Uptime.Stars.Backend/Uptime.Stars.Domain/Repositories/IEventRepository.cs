using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Domain.Repositories;
public interface IEventRepository
{
    Task AddAsync(Event @event, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Event>> GetLastByIdAsync(Guid monitorId, int limit = 50, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Event>> GetLastByIdSinceAsync(Guid monitorId, DateTime sinceDateTime, CancellationToken cancellationToken = default);
}