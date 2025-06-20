using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Persistence.Repositories;
internal sealed class MonitorRepository(IDbContext dbContext) : IMonitorRepository
{
    public async Task AddAsync(ComponentMonitor monitor, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<ComponentMonitor>().AddAsync(monitor, cancellationToken);
    }

    public async Task<ComponentMonitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<ComponentMonitor>().FindAsync([id], cancellationToken);
    }
}