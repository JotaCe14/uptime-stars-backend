using Microsoft.EntityFrameworkCore;
using System.Threading;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Persistence.Repositories;

internal sealed class EventRepository(IDbContext dbContext) : IEventRepository
{
    public async Task AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<Event>().AddAsync(@event, cancellationToken);
    }

    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Event>().FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyCollection<Event>> GetLastByMonitorIdAsync(Guid monitorId, int limit = 20, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Event>()
            .Where(entity => entity.MonitorId == monitorId)
            .OrderByDescending(entity => entity.TimestampUtc)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Event>> GetLastImportantByMonitorIdAsync(Guid monitorId, int limit = 20, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Event>()
            .Where(entity => entity.MonitorId == monitorId && entity.IsImportant)
            .OrderByDescending(entity => entity.TimestampUtc)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Event>> GetLastByMonitorIdSinceAsync(Guid monitorId, DateTime sinceDateTime, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Event>()
            .Where(entity => entity.MonitorId == monitorId && entity.TimestampUtc >= sinceDateTime)
            .ToListAsync(cancellationToken);
    }
}