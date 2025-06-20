using Microsoft.EntityFrameworkCore;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Persistence.Repositories;
internal sealed class GroupRepository(IDbContext dbContext) : IGroupRepository
{
    public async Task AddAsync(Group group, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<Group>().AddAsync(group, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<Group>()
            .Where(g => g.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Group>().FindAsync([id], cancellationToken);
    }
}
