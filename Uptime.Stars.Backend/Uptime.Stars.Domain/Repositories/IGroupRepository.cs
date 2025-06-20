using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Domain.Repositories;
public interface IGroupRepository
{
    Task AddAsync(Group group, CancellationToken cancellationToken = default);
    Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}